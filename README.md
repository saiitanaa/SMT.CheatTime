# SMT.CheatTime

**Overview**

A toolkit for Supermarket Together that handles real-time resource modification, physics overrides, and custom in-game menus.


**Functionality**

- [+] - Add Money
- [+] - Add Franchise Points
- [=] - No-Clip
- [+] - Speed Hack (x1 - x10)
- [?] - Random Cheat "I am lucky ?"
- [+] - Anti-Jail
- [+] - Grab all steals
- [+] - Disable barrier
- [+] - Free DLC
- [+] - Random Teleport

**Usage**

Download latest release

Extract Archive

Start `SMT-CheatTime.py` and press 1

**How it works**

* **Initialization**: Registers a custom plugin (`com.saysaa.smt_cheattime` vX.X) via **BepInEx** into the game process during startup.
* **Data Access & State Manipulation**:
* Modifies `GameData.Instance` values directly (e.g., `NetworkgameFunds` and `NetworkgameFranchisePoints`) to manipulate in-game money and points in real-time.
* Leverages **Harmony Reflection (`Traverse`)** to bypass private field restrictions and force UI enabling for content unlocks (such as `isCool` and `canPlace` on `Builder_Main`).
* Triggers network commands directly on objects, such as invoking `CmdRecoverStolenProduct()` on all active `StolenProductSpawn` instances.


* **Engine Integration**:
* `Update`: Monitors key presses (`F3` to toggle menu/cursor lock, `W/A/S/D` or `Z/Q` for directional movement) and calculates No-Clip positional offsets based on the main camera's orientation. Also updates the display timer for temporary notification messages.
* `OnGUI`: Renders the floating menu interface (`SMT.CheatTime! vX.X`) with tabs (*Cheats* & *Settings*), buttons, status labels, dynamic scrolling, and temporary screen overlay messages.
* **Physics & Components**:
* **No-Clip Mode**: Uses reflection via `GetType().GetProperty()` to dynamically disable or enable the `enabled` property of `Collider` components and the `useGravity` property of `Rigidbody` components.
* **Teleportation**: Temporarily disables `CharacterController` instances to bypass internal collision checks while shifting transform positions.
* **Environment**: Toggles global `GameObject` visibility/activity states to disable map colliders (such as boundaries or jail rooms) and reveal hidden DLC UI containers.


*Disclaimer: For private/educational use only. Use at your own risk.*
