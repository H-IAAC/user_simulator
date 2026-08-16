import os
import numpy as np
import subprocess
import shlex
import socket
import io

import sys

from imusim.platforms.imus import IdealIMU, Orient3IMU
from imusim.simulation.base import Simulation
from imusim.behaviours.imu import BasicIMUBehaviour
from imusim.io.bvh import BVHLoader
from imusim.trajectories.rigid_body import SplinedBodyModel
from imusim.environment.base import Environment
from imusim.simulation.calibrators import ScaleAndOffsetCalibrator
from imusim.trajectories.offset import OffsetTrajectory
from imusim.maths.quaternions import Quaternion

import argparse
import pickle as cp
import numpy as np

hostname = socket.gethostname()

import matplotlib
matplotlib.use('Agg')
#matplotlib.use('TKAgg')
import matplotlib.pyplot as plt

import yaml

# change for different skeletons 
joint_mapping_unity = {
    'Left_hip':      'LeftUpperLeg',
    'Left_knee':     'LeftLowerLeg',
    'Left_ankle':    'LeftFoot',
    'Right_hip':     'RightUpperLeg',
    'Right_knee':    'RightLowerLeg',
    'Right_ankle':   'RightFoot',
    'Spine1':        'Spine',
    'Spine2':        'Chest',
    'Spine3':        'UpperChest',
    'Head':          'Head',
    'Left_shoulder': 'LeftUpperArm',
    'Left_elbow':    'LeftLowerArm',
    'Left_wrist':    'LeftHand',
    'Right_shoulder':'RightUpperArm',
    'Right_elbow':   'RightLowerArm',
    'Right_wrist':   'RightHand',
}

_samplingPeriod = 0.
calibSamples = 1000
calibRotVel = 20

def plot_imu(sensor, config):
    dir_sensor_pID = config['dir_result']
    if not os.path.exists(dir_sensor_pID):
        os.makedirs(dir_sensor_pID)

    acc = sensor['acc']
    gyro = sensor['gyro']

    for joint in acc:
        if joint not in gyro:
            continue

        len_seq = np.amin((acc[joint].shape[0], gyro[joint].shape[0]))
        acc_joint = acc[joint][:len_seq]
        gyro_joint = gyro[joint][:len_seq]
        ts = np.arange(len_seq)

        fig = plt.figure(figsize=(10,5))
        fig.suptitle('{}'.format(joint))

        ax = fig.add_subplot(6,1,1)
        ax.set_title('Accelerometer')
        ax.plot(ts, acc_joint[:,0], 'r')
        ax.set_ylabel('x')
        ax = fig.add_subplot(6,1,2)
        ax.plot(ts, acc_joint[:,1], 'g')
        ax.set_ylabel('y')
        ax = fig.add_subplot(6,1,3)
        ax.plot(ts, acc_joint[:,2], 'b')
        ax.set_ylabel('z')

        ax = fig.add_subplot(6,1,4)
        ax.set_title('Gyroscope')
        ax.plot(ts, gyro_joint[:,0], 'r')
        ax.set_ylabel('x')
        ax = fig.add_subplot(6,1,5)
        ax.plot(ts, gyro_joint[:,1], 'g')
        ax.set_ylabel('y')
        ax = fig.add_subplot(6,1,6)
        ax.plot(ts, gyro_joint[:,2], 'b')
        ax.set_ylabel('z')

        file_savefig = os.path.join(dir_sensor_pID, '{}.png'.format(joint))
        fig.subplots_adjust(hspace=.5)
        plt.savefig(file_savefig)
        print('save in ...', file_savefig)
        plt.close(fig)

def build_sensor_trajectories(splinedModel, config):
    """
    Monta a lista unificada de sensores a simular: um para cada junta
    de joint_mapping_unity (trajetoria = a propria junta) e um para
    cada entrada em config['custom_sensors'] (trajetoria = OffsetTrajectory
    presa a uma junta pai, deslocada por posicao/rotacao).
 
    Retorna uma lista de tuplas (sensor_name, trajectory).
    """
    sensor_trajectories = []
 
    # sensores nas juntas (comportamento original)
    for sensor_name, joint_name in joint_mapping_unity.items():
        joint_traj = splinedModel.getJoint(joint_name)
        sensor_trajectories.append((sensor_name, joint_traj))
 
    # sensores customizados (celular, smartwatch, etc.), presos a uma
    # junta pai com um offset de posicao/rotacao
    custom_sensors = config.get('custom_sensors', []) or []
    for custom in custom_sensors:
        sensor_name = custom['name']
        parent_joint_name = custom['joint_name_bvh']
        pos = custom.get('position_offset_xyz_m', [0.0, 0.0, 0.0])
        rot = custom.get('rotation_offset_zyx_deg', [0.0, 0.0, 0.0])
 
        parent_joint = splinedModel.getJoint(parent_joint_name)
        position_offset = np.array(pos, dtype=float).reshape(3, 1)
        rotation_offset = Quaternion.fromEuler(rot, order='zyx', inDegrees=True)
 
        custom_traj = OffsetTrajectory(parent_joint, position_offset, rotation_offset)
        sensor_trajectories.append((sensor_name, custom_traj))
 
    return sensor_trajectories

def extract_vir_imu(bvh_source):

    with open('config.yaml', 'r') as f:
        config = yaml.safe_load(f)

    dir_result = config['dir_result']
    if not os.path.exists(dir_result):
        os.makedirs(dir_result)

    file_acc = config['file_acc']
    file_gyro = config['file_gyro']

    sensor = {}
 
    # Extact virtual sensor with imusim
    # updated to python 3 version
    path_acc = file_acc
    path_gyro = file_gyro
 
    # load mocap — bvh_source pode ser str (caminho) ou file-like
    conversionFactor = 1
    if isinstance(bvh_source, str):
        with open(bvh_source, 'r') as f:
            loader = BVHLoader(f, conversionFactor)
        # print('load mocap from ...', bvh_source)
    else:
        loader = BVHLoader(bvh_source, conversionFactor)
        # print('load mocap from in-memory bvh data')
    loader._readHeader()
    loader._readMotionData()
    model = loader.model
 
    # spline intrepolation
    splinedModel = SplinedBodyModel(model)
    startTime = splinedModel.startTime
    endTime = splinedModel.endTime
 
    if _samplingPeriod == 0.:
        samplingPeriod = (endTime - startTime) / loader.frameCount
    else:
        samplingPeriod = _samplingPeriod
    print('frameCount:', loader.frameCount)
    print('samplingPeriod:', samplingPeriod)
 
    # lista unificada: [(sensor_name, trajectory), ...]
    # inclui juntas de joint_mapping_unity + sensores customizados do config
    sensor_trajectories = build_sensor_trajectories(splinedModel, config)
 
    if config["version"] == 'ideal':
        print('Simulating ideal IMU.')
 
        # set simulation
        sim = Simulation()
        sim.time = startTime
 
        # run simulation
        dict_imu = {}
        for sensor_name, trajectory in sensor_trajectories:
            imu = IdealIMU()
            imu.simulation = sim
            imu.trajectory = trajectory
 
            BasicIMUBehaviour(imu, samplingPeriod)
 
            dict_imu[sensor_name] = imu
 
        sim.run(endTime)
 
    elif config["version"] == 'sim':
        print('Simulating Orient3IMU.')
 
        # set simulation
        env = Environment()
        calibrator = ScaleAndOffsetCalibrator(env, calibSamples, samplingPeriod, calibRotVel)
        sim = Simulation(environment=env)
        sim.time = startTime
 
        # run simulation
        dict_imu = {}
        for sensor_name, trajectory in sensor_trajectories:
 
            imu = Orient3IMU()
            calibration = calibrator.calibrate(imu)
            print('imu calibration:', sensor_name)
 
            imu.simulation = sim
            imu.trajectory = trajectory
 
            BasicIMUBehaviour(imu, samplingPeriod, calibration, initialTime=sim.time)
 
            dict_imu[sensor_name] = imu
 
        sim.run(endTime)
 
    # collect sensor values
    acc_seq = {}
    gyro_seq = {}
    for sensor_name, _ in sensor_trajectories:
        imu = dict_imu[sensor_name]
 
        if config["version"] == 'ideal':
            acc_seq[sensor_name] = imu.accelerometer.rawMeasurements.values.T
            gyro_seq[sensor_name] = imu.gyroscope.rawMeasurements.values.T
        elif config["version"] == 'sim':
            acc_seq[sensor_name] = imu.accelerometer.calibratedMeasurements.values.T
            gyro_seq[sensor_name] = imu.gyroscope.calibratedMeasurements.values.T
 
        print(sensor_name, acc_seq[sensor_name].shape, gyro_seq[sensor_name].shape)
 
    # save
    np.savez(path_acc, **acc_seq)
    print('save in ...', path_acc)
    np.savez(path_gyro, **gyro_seq)
    print('save in ...', path_gyro)
 
    # -----------------------------
 
    acc = acc_seq
    gyro = gyro_seq
 
    sensor = {
        'acc': acc,
        'gyro': gyro}
 
    if config.get('plot', False):
        plot_imu(sensor, config)
 
    return sensor


# if __name__ == '__main__':

#     with open('config.yaml', 'r') as f:
#         config = yaml.safe_load(f)

#     bvh_source = config['file_bvh']
#     file_acc = config['file_acc']
#     file_gyro = config['file_gyro']

#     dir_result = config['dir_result']
#     if not os.path.exists(dir_result):
#         os.makedirs(dir_result)

#     sensor = extract_vir_imu(bvh_source)