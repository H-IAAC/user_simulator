import io
import time
import signal
import cst_python as cst
from cst_python.memory_storage import MemoryStorageCodelet

from sensor import extract_vir_imu

class RedisReader:

    mind_name = "default_mind"
    node_name = "python_reader"
    redis_host = "localhost"
    redis_port = 6379
    
    time_step = 50 # milliseconds

    def __init__(self):
        
        self.mind = cst.Mind()

        self.bvh_pose = self.mind.create_memory_object("bvh_pose", "")
        self.sensor_config = self.mind.create_memory_object("sensor_config", "")
        self.simulation_running = self.mind.create_memory_object("simulation_running")

        mscodelet = MemoryStorageCodelet(self.mind, node_name=self.node_name, mind_name=self.mind_name, host=self.redis_host, port=self.redis_port)
        mscodelet.time_step = self.time_step

        self.mind.insert_codelet(mscodelet)
        self.mind.start()

        self.memories = {
            "bvh_pose": self.bvh_pose,
            "sensor_config": self.sensor_config,
            "simulation_running": self.simulation_running,
        }

        # guarda o último timestamp visto de cada memória, pra só reagir quando mudar de fato
        last_timestamps = {name: -1 for name in self.memories}

        running = True
        def handle_sigint(signum, frame):
            global running
            running = False

        signal.signal(signal.SIGINT, handle_sigint)

        try:
            while running:
                # print(self.simulation_running)
                for name, mem in self.memories.items():
                    ts = mem.get_timestamp()
                    if ts != last_timestamps[name]:
                        last_timestamps[name] = ts
                        # print(f"[{name}]-> {mem.get_info()}")

                if self.simulation_running.get_info() == False:
                    print("Simulation finished. Waiting for BVH data to sync...")

                    # Aguarda o MemoryStorageCodelet terminar de sincronizar o BVH
                    # (evita race condition: simulation_running chega antes do bvh_pose)
                    bvh_ts = self.bvh_pose.get_timestamp()
                    while self.bvh_pose.get_timestamp() == bvh_ts:
                        time.sleep(0.01)

                    bvh = self.bvh_pose.get_info()
                    if not bvh or bvh == "":
                        print("ERROR: BVH data is empty after sync")
                        break

                    print(f"BVH data received ({len(bvh)} chars). Extracting virtual IMU data...")
                    extract_vir_imu(io.StringIO(bvh))
                    # extract_vir_imu(bvh)

                    print("Virtual IMU data extracted. Exiting...")
                    break

                time.sleep(0.05)  # 50 ms
        finally:
            print("Encerrando...")
            mscodelet.stop()
            
if __name__ == "__main__":
    reader = RedisReader()