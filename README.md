[![GitHub release](https://flat.badgen.net/github/release/gamendegamer321/Reflektor/)](https://github.com/gamendegamer321/Reflektor/releases/latest)
[![KSP Version](https://flat.badgen.net/static/Game%20Version/v0.2.1+)](https://github.com/gamendegamer321/Reflektor)
[![SpaceWarp Version](https://flat.badgen.net/static/SpaceWarp%20Version/v1.9.5+)](https://github.com/SpaceWarpDev/SpaceWarp)
[![Original](https://flat.badgen.net/static/Original/coldrifting?icon=github)](https://github.com/coldrifting/Reflektor)
[![License](https://flat.badgen.net/github/license/gamendegamer321/Reflektor/)](https://github.com/gamendegamer321/Reflektor/blob/master/LICENSE)

> [!NOTE]
> This repository is a fork of [coldrifting/Reflektor](https://github.com/coldrifting/Reflektor).
> 
> This README has also been modified, view the original [here](https://github.com/coldrifting/Reflektor).

# About R.E.F.L.E.K.T.O.R

![Reflektor2](https://github.com/coldrifting/Reflektor/assets/31460040/3b1cc2ec-1d7f-4360-9c0a-710d4e6b323e)

Reflection Enhanced Fairly Lightweight Editor for KSP Two Objects and Rectangles

All jokes aside, a quicker, lighter weight alternative to Unity Explorer for KSP 2.

# Installation

> [!IMPORTANT]
> **Required dependencies:**
> - [SpaceWarp](https://spacedock.info/mod/3277/Space%20Warp%20+%20BepInEx) (v1.9.5+)

Download the release from the latest release
from [GitHub](https://github.com/gamendegamer321/Reflektor/releases/latest).

Place the downloaded assembly in the <code>BepInEx/Plugins</code> folder.
You can find the KSP 2 root folder by right-clicking the game in your steam library,
selecting <code>Manage</code> and then clicking <code>Browse local files</code>.

# Usage

- Browse the GameObject tree and disable objects by right clicking in the browser (Shift + Alt + Q by default).
- Fire a raycast to see what gameobjects are behind objects on the screen (Shift + Alt + R by default).
- Edit Fields/Properties of objects and invoke parameterless methods in the inspector (Shift + Alt + E by default).
- The new and improved editor supports editing of strings, numbers, booleans, enums (including multi-flag support),
  colors, vectors, etc,
  even inside of lists and dictionaries.
- The logs view contains the BepInEx logs, allowing them to be viewed live in the game.
- Logs can be filtered on the level of the message and on message contents.
- The messages view contains a history of each <code>MessageCenterMessage</code> published in the <code>
  MessageCenter</code>.
