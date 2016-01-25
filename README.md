# CodeSync
CodeSync is a way to migrate a folder of Lua code to ROBLOX Studio and keep the Studio version up-to-date. This lets you use external editors and version control systems with ROBLOX games (relatively) easily.

## Setup

1. Install the plugin from [here](http://www.roblox.com/CodeSync-item?id=348031028).
2. Get the server [here](https://github.com/MemoryPenguin/CodeSync/releases/latest).
3. Create a config file as described below.
4. Run the server with `codesync <config_file>`.
5. Open the plugin in Studio, enter the port, and press 'Start sync'
6. Have fun!

## Config file
CodeSync uses a simple JSON config file that looks something like this:

```json
{
  "Port": 4114,
  "AllowExternalRequests": false,
  "Path": "D:\\Documents\\CodeSync Test",
  "SyncLocation": "game.ReplicatedStorage.TestProject"
}
```

Each key is required. You can find an example [here](config-example.json).

### Port
This is the port that the CodeSync server runs on, and must be a number between 1 and 65535. It can't be occupied by other applications, and if you want to allow other computers to access the server, it needs to be accessible by the outside world.

### AllowExternalRequests
Setting this to true will cause the CodeSync server to respond to requests that didn't originate from the local machine.

### Path
This is the path that the CodeSync server exposes.

### SyncLocation
This is where the files will be synced in ROBLOX. In this example, it's putting them in the child of `game.ReplicatedStorage` named `TestProject`. If TestProject (or anything else in the path) doesn't exist when you start syncing, it will create `Folder` instances to match this path.

## File Names
CodeSync determines what type of object (`LocalScript`, `ModuleScript`, or `Script`) a file is based on its name. It's simple:

* Files that are in the format `Name.module.extension` are considered `ModuleScripts`.
* Files that are in the format `Name.local.extension` are considered `LocalScripts`.
* Anything else is considered a `Script`. You may choose to name those differently; it will not affect this process.