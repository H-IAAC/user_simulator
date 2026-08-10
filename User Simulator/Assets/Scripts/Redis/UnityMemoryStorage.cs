using System;
using UnityEngine;
using HIAAC.CstUnity.Core.Entities;
using System.Collections;
using System.Collections.Generic;
using HIAAC.CstUnity.MemoryStorage;

using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace HIAAC.CstUnity
{
    //C# equivalent of https://h-iaac.github.io/CST-Python/_build/html/_examples/Memory%20Storage.html
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

        public Memory BvhPose => bvh_pose;
        public Memory SensorConfig => sensor_cofig;
        public Memory SimulationRunning => simulation_running;


        void Start()
        {
            var muxer = ConnectionMultiplexer.Connect("localhost,allowAdmin=true");
            var server = muxer.GetServer(redisHost, redisPort);
            server.FlushAllDatabases();

            mind = new Mind();

            bvh_pose = mind.createMemoryObject("bvh_pose", "");
            sensor_cofig = mind.createMemoryObject("sensor_config", "");
            simulation_running = mind.createMemoryObject("simulation_running", true);

            memoryStorageCodelet = new MemoryStorageCodelet(mind, NodeName, MindName, 0.5, $"{redisHost}:{redisPort},abortConnect=true");
        }

        public void SetBvhPose(string bvhData)
        {
            bvh_pose.setI(bvhData);
            Debug.Log($"BVH data set in memory storage: {bvhData}");
        }

        public void SetSensorConfig(string sensorConfig)
        {
            sensor_cofig.setI(sensorConfig);
        }

        public void SetSimulationRunning(bool running)
        {
            simulation_running.setI(running);
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