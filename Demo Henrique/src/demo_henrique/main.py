
import time

import cst_python as cst
from cst_python.memory_storage import MemoryStorageCodelet

from .random_action_selector import RandomActionSelector

if __name__ == "__main__":
    mind = cst.Mind()

    surrounding_actions_mo = mind.create_memory_object("SurroundingActions")
    skill_manifest_mo = mind.create_memory_object("SkillManifest")
    action_command_mo = mind.create_memory_object("ActionCommand")
    action_status_mo = mind.create_memory_object("ActionStatus")

    assert surrounding_actions_mo is not None
    assert skill_manifest_mo is not None
    assert action_status_mo is not None
    assert action_command_mo is not None

    action_selector = RandomActionSelector()
    action_selector.add_input(surrounding_actions_mo)
    action_selector.add_input(skill_manifest_mo)
    action_selector.add_input(action_status_mo)
    action_selector.add_output(action_command_mo)
    mind.insert_codelet(action_selector)

    mscodelet = MemoryStorageCodelet(mind, host="127.0.0.1")
    mscodelet.time_step = 50
    mind.insert_codelet(mscodelet)

    # Wait for Unity node
    if mscodelet._client.scard(f"{mscodelet._mind_name}:nodes") == 1:
        mscodelet._client.flushall()
        raise RuntimeError("Unity node must be started before Python node.")

    mind.start()

    while True:
        pass
