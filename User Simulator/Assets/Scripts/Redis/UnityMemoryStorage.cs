using System;
using UnityEngine;
using HIAAC.CstUnity.Core.Entities;
using System.Collections;
using System.Collections.Generic;
using HIAAC.CstUnity.MemoryStorage;

using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace HIAAC.CstUnity
{
    //C# equivalent of https://h-iaac.github.io/CST-Python/_build/html/_examples/Memory%20Storage.html

    // Runs before other components (e.g. SkillsExporter) so the memories exist when they read/write them
    [DefaultExecutionOrder(-100)]
    public class UnityMemoryStorage : MonoBehaviour
    {
        [Header("Redis")]
        [SerializeField] private string redisHost = "localhost";
        [SerializeField] private int redisPort = 6379;

        private const string MindName = "default_mind";
        private const string NodeName = "memory_storage";

        private Mind mind;
        private MemoryStorageCodelet memoryStorageCodelet;

        private Memory bvh_pose;
        private Memory sensor_cofig;
        private Memory simulation_running;
        private Memory action_command;
        private Memory action_status;
        private Memory skill_manifest;

        public Memory BvhPose => bvh_pose;
        public Memory SensorConfig => sensor_cofig;
        public Memory SimulationRunning => simulation_running;
        public Memory ActionCommand => action_command;
        public Memory ActionStatus => action_status;
        public Memory SkillManifest => skill_manifest;

        private ConnectionMultiplexer _muxer;

        void Start()
        {
            _muxer = ConnectionMultiplexer.Connect("localhost,allowAdmin=true");
            var server = _muxer.GetServer(redisHost, redisPort);
            server.FlushAllDatabases();

            mind = new Mind();

            bvh_pose = mind.createMemoryObject("bvh_pose", "");
            sensor_cofig = mind.createMemoryObject("sensor_config", "");
            simulation_running = mind.createMemoryObject("simulation_running", true);
            action_command = mind.createMemoryObject("action_command", "");
            action_status = mind.createMemoryObject("action_status", "");
            skill_manifest = mind.createMemoryObject("skill_manifest", "");

            memoryStorageCodelet = new MemoryStorageCodelet(mind, NodeName, MindName, 0.5, $"{redisHost}:{redisPort},abortConnect=true");
            mind.insertCodelet(memoryStorageCodelet);
            mind.start();
        }

        public void SetBvhPose(string bvhData)
        {
            bvh_pose.setI(bvhData);
        }

        public void SetSensorConfig(string sensorConfig)
        {
            sensor_cofig.setI(sensorConfig);
        }

        public void SetSimulationRunning(bool running)
        {
            simulation_running.setI(running);
        }

        public void SetSkillManifest(string manifest)
        {
            skill_manifest.setI(manifest);
        }

        public void SetActionStatus(string status)
        {
            action_status.setI(status);
        }

        public void ClearActionCommand()
        {
            action_command.setI("");
        }

        public object GetActionCommand()
        {
            return action_command.getI();
        }

        public object GetBvhPose()
        {
            return bvh_pose.getI();
        }

        public object GetSensorConfig()
        {
            return sensor_cofig.getI();
        }

        public object GetSimulationRunning()
        {
            return simulation_running.getI();
        }
    }
}