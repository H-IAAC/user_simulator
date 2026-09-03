from dataclasses import dataclass, field
from typing import Any


@dataclass
class CommandEntry:
    Skill: str  # noqa
    Parameters: dict[str, Any] = field(default_factory=dict)  # noqa
