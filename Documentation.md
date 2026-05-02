# PvZRSkinPicker Documentation
### Custom skin pack format

A custom skin pack is defined by a JSON manifest file that follows a specific schema.

- **Current format version:** `1` (The loader will warn if it mismatches, but won't block loading).
- All GUIDs can be generated using standard GUID generation tools (e.g., https://guidgenerator.com/)
- Vanilla assets can be extracted using [UABEA](https://github.com/nesrak1/UABEA)

```text
Replanted_Data/StreamingAssets/aa/StandaloneWindows64/spineassets_assets_assets/art/characters/spine.bundle
```

- **Important (Texture Swaps):** These custom skins are primarily **Texture swaps**. You can simply copy the original game textures, edit the `.png`, and override the texture asset at runtime. If you want to replace the whole animation, you can include `.skel` & `.atlas` files (requires Spine v4.2.x).
- **Debug Packs:** The provided `DebugZombiesPack` and `DebugPlantsPack` are primarily **manifest templates**. They contain the full list of supported internal types and provide the folder structure, but they do not include the textures (`.png`).

### Root structure

The root of the manifest contains the format version and two main sections:

| Field            | Type             | Description                                                                |
|------------------|------------------|----------------------------------------------------------------------------|
| `format_version` | `integer`        | The format version of this manifest. Must be `1` for current compatibility |
| `header`         | `SkinPackHeader` | Metadata about the skin pack (name, id, version, authors)                  |
| `skins`          | `SkinCatalog`    | Container for all available skins in the pack                              |

### SkinPackHeader

| Field     | Type       | Constraints                                                                                | Description                                                       |
|-----------|------------|--------------------------------------------------------------------------------------------|-------------------------------------------------------------------|
| `name`    | `string`   | <ul><li>Max 30 chars</li><li>Printable ASCII only</li></ul>                                | Display name of the skin pack.                                    |
| `id`      | `GUID`     | <ul><li>Must not be empty GUID</li></ul>                                                   | Unique identifier for the skin pack.                              |
| `version` | `integer`  | <ul><li>Must be > 0</li></ul>                                                              | Version number of the skin pack. Increment when releasing updates |
| `authors` | `string[]` | <ul><li>Max 30 chars each</li><li>Printable ASCII only</li><li>At least 1 author</li></ul> | List of authors/creators of the skin pack.                        |

### SkinCatalog

| Field       | Type         | Description                                                  |
|-------------|--------------|--------------------------------------------------------------|
| `[plants]`  | `SkinEntry[]`| Array of available plant skins. All skin ids must be unique  |
| `[zombies]` | `SkinEntry[]`| Array of available zombie skins. All skin ids must be unique |

### SkinEntry

| Field         | Type                   | Constraints                                                                         | Description                                                                                               |
|---------------|------------------------|-------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------|
| `type`        | `Plant- or ZombieType` | <ul><li>Must be a valid PlantType or ZombieType</li></ul>                           | The internal type of entity of the skin                                                                   |
| `name`        | `string`               | <ul><li>Max 30 chars</li><li>Printable ASCII only</li></ul>                         | Display name of the skin                                                                                  |
| `id`          | `GUID`                 | <ul><li>Must not be empty GUID</li></ul>                                            | Unique identifier for this skin                                                                           |
| `directory`   | `string`               | <ul><li>Max 30 chars</li><li>`a–z` `A–Z` `0–9`</li><li>`-` `_`</li></ul>            | Name of directory containing the skin files                                                               |
| `[pixelated]` | `boolean`              | <ul><li>`true` or `false`</li></ul>                                                 | `true` disables smoothing (sharp pixels, useful for pixel art skins), `false` enables smoothing (default) |

### Example Manifest

```json
{
    "format_version": 1,
    "header": {
        "name": "Example skin pack",
        "id": "00000000-0000-0000-0000-000000000000",
        "version": 1,
        "authors": [
            "Author A",
            "Author B"
        ]
    },
    "skins": {
        "plants": [
            {
                "type": "Peashooter",
                "name": "My Peashooter",
                "id": "00000000-0000-0000-0000-000000000000",
                "directory": "CustomPeashooter",
                "pixelated": false
            }
        ],
        "zombies": [
            {
                "type": "Normal",
                "name": "Cool Zombie",
                "id": "00000000-0000-0000-0000-000000000000",
                "directory": "CoolZombie",
                "pixelated": true
            }
        ]
    }
}
```

- File structure:

```text
📁 PVZ Replanted/
└── 📁 UserData/
    └── 📁 PvZRSkinPicker/
        └── 📁 SkinPacks/
            └── 📁 MySkinPack-V1/
                ├── 🔢 manifest.json
                └── 📁 CustomPeashooter/
                    ├── 🖼️ skin.png
                    ├── 📄 skin.atlas
                    └── 🦴 skin.skel
```

Texture (`.png`), atlas (`.atlas`) & skeleton (`.skel`) files must be named `skin.{ext}`.

### Plant types

| 1             | 2          | 3            | 4            | 5           | 6           | 7             |
| ------------- | ---------- | ------------ | ------------ | ----------- | ----------- | ------------- |
| Peashooter    | Sunflower  | CherryBomb   | Wallnut      | PotatoMine  | SnowPea     | Chomper       |
| Repeater      | Puffshroom | Sunshroom    | Fumeshroom   | GraveBuster | Hypnoshroom | Scaredyshroom |
| IceShroom     | DoomShroom | LilyPad      | Squash       | Threepeater | TangleKelp  | Jalapeno      |
| Spikeweed     | Torchwood  | Tallnut      | SeaShroom    | Plantern    | Cactus      | Blover        |
| SplitPea      | Starfruit  | Pumpkinshell | Magnetshroom | Cabbagepult | FlowerPot   | Kernelpult    |
| InstantCoffee | Garlic     | Umbrella     | Marigold     | Melonpult   | GatlingPea  | TwinSunflower |
| Gloomshroom   | Cattail    | WinterMelon  | GoldMagnet   | Spikerock   | CobCannon   | Imitater      |

### Zombie types

| 1           | 2            | 3            | 4            | 5           | 6           | 7             |
| ----------- | ------------ | ------------ | ------------ | ----------- | ----------- | ------------- |
| Normal      | Flag         | TrafficCone  | Polevaulter  | Pail        | Newspaper   | Door          |
| Football    | Dancer       | BackupDancer | Snorkel      | Zamboni     | Bobsled     | DolphinRider  |
| JackInTheBox| Balloon      | Digger       | Pogo         | Yeti        | Bungee      | Ladder        |
| Catapult    | Gargantuar   | Imp          | Boss         |             |             |               |

### Versioning & Packaging
- Directory naming convention: Skin pack directories should use the `-V{N}` suffix to indicate the version (e.g. `MySkinPack-V1`).
- The `version` field in the `SkinPackHeader` must match the `-V{N}` suffix in the directory name.
- To simplify installation for users, skin packs should be distributed so the archive already contains the required folder structure `UserData/PvZRSkinPicker/SkinPacks/MySkinPack-V1`.


