from dataclasses import dataclass

from .command_entry import CommandEntry


@dataclass
class CommandPayload:
    Id: int  # noqa
    Commands: list[CommandEntry]  # noqa
