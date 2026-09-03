import json
import random
from dataclasses import asdict
from typing import cast

from cst_python.core.entities import Codelet, Memory

from .command_entry import CommandEntry
from .command_payload import CommandPayload


class RandomActionSelector(Codelet):
    def __init__(self) -> None:
        super().__init__()
        self.action_command_mo: Memory | None
        self.action_status_mo: Memory | None
        self.surrounding_actions_mo: Memory | None
        self.skill_manifest_mo: Memory | None

        self.commands: dict[str, list[CommandEntry]] = {}
        self.actions: list[str] = []
        self.last_action = ""
        self.last_id = 0
        self.last_payload_size = 0

        self.velocity = 3.5

    def _create_commands(self, surrounding_actions: dict) -> dict[str, list[CommandEntry]]:
        result = {}

        for action in surrounding_actions:
            commands = []
            position = action["originPosition"]
            name = action["name"]

            commands.append(CommandEntry("walk_to",
                                         {"destination": position, "velocity": self.velocity}))

            if name == "Trabalhar":
                commands.append(CommandEntry("work"))

            elif name == "Beber café":
                commands.append(CommandEntry("drink_coffee"))

            elif name == "Usar":
                commands.append(CommandEntry("use_bathroom"))

            result[name] = commands

        return result

    def access_memory_objects(self) -> None:
        self.surrounding_actions_mo = self.get_input(name="SurroundingActions")
        self.skill_manifest_mo = self.get_input(name="SkillManifest")
        self.action_status_mo = self.get_input(name="ActionStatus")

        self.action_command_mo = self.get_output(name="ActionCommand")

    def calculate_activation(self) -> None:  # noqa
        pass

    def proc(self) -> None:
        self.surrounding_actions_mo = cast(Memory, self.surrounding_actions_mo)
        self.skill_manifest_mo = cast(Memory, self.skill_manifest_mo)
        self.action_status_mo = cast(Memory, self.action_status_mo)
        self.action_command_mo = cast(Memory, self.action_command_mo)

        if len(self.actions) == 0:
            if self.surrounding_actions_mo.get_info() == "" or self.skill_manifest_mo.get_info() == "":
                return

            self.commands = self._create_commands(
                self.surrounding_actions_mo.get_info())
            self.actions = list(self.commands.keys())

            action = next(iter(self.commands.values()))
            payload = CommandPayload(1, action)
            self.action_command_mo.set_info(asdict(payload))

            self.last_id = 1
            self.last_payload_size = 2
            self.last_action = action

        status = self.action_status_mo.get_info()

        if status == "":
            return

        status = json.loads(status)

        if status["state"] != "completed" or status["index"] < self.last_payload_size-1:
            return

        action = random.choice(self.actions)  # noqa
        while action == self.last_action and len(self.actions) > 1:
            action = random.choice(self.actions)  # noqa

        command = self.commands[action]

        self.last_id += 1
        self.last_payload_size = len(command)
        self.last_action = action
        payload = CommandPayload(self.last_id, command)

        self.action_command_mo.set_info(asdict(payload))
