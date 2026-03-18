# Custom skin pack format

A custom skin pack is defined by a JSON manifest file that follows a specific schema

- **Current format version:** `1`

- All GUIDs can be generated using standard GUID generation tools (e.g., https://guidgenerator.com/)

- All fields are required except those enclosed in square brackets: `[Field]`

- Vanilla assets can be extracted using [UABEA](https://github.com/nesrak1/UABEA)

```
Replanted_Data/StreamingAssets/aa/StandaloneWindows64/spineassets_assets_assets/art/characters/spine.bundle
```

- Creating/editing `.skel` & `.atlas` files requires [Spine](https://en.esotericsoftware.com/), Replanted uses `v4.2`

- Modding server: [![Discord](https://img.shields.io/badge/Discord-%235865F2.svg?&logo=discord&logoColor=white)](https://discord.gg/Z6Ms2TkkQ9)

## Root structure

The root of the manifest contains the format version and two main sections:

| Field            | Type             | Description                                                                |
|------------------|------------------|----------------------------------------------------------------------------|
| `format_version` | `integer`        | The format version of this manifest. Must be `1` for current compatibility |
| `header`         | `SkinPackHeader` | Metadata about the skin pack (name, id, version, authors)                  |
| `skins`          | `SkinCatalog`    | Container for all available skins in the pack                              |

## SkinPackHeader

Metadata and identification information for the skin pack

| Field     | Type       | Constraints                                                                                | Description                                                       |
|-----------|------------|--------------------------------------------------------------------------------------------|-------------------------------------------------------------------|
| `name`    | `string`   | <ul><li>Max 30 chars</li><li>Printable ASCII only</li></ul>                                | Display name of the skin pack.                                    |
| `id`      | `GUID`     | <ul><li>Must not be empty GUID</li></ul>                                                   | Unique identifier for the skin pack.                              |
| `version` | `integer`  | <ul><li>Must be > 0</li></ul>                                                              | Version number of the skin pack. Increment when releasing updates |
| `authors` | `string[]` | <ul><li>Max 30 chars each</li><li>Printable ASCII only</li><li>At least 1 author</li></ul> | List of authors/creators of the skin pack.                        |

## SkinCatalog

Container for all available skins in the pack

| Field      | Type         | Description                                                 |
|------------|--------------|-------------------------------------------------------------|
| `[plants]` | `SkinEntry[]`| Array of available plant skins. All skin ids must be unique |

## SkinEntry

Represents a single skin within the pack

| Field         | Type        | Constraints                                                              | Description                                                                                               |
|---------------|-------------|--------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------|
| `type`        | `PlantType` | <ul><li>Must be a valid [PlantType](#plant-types)</li></ul>              | The type of plant of the skin                                                                             |
| `name`        | `string`    | <ul><li>Max 30 chars</li><li>Printable ASCII only</li></ul>              | Display name of the skin                                                                                  |
| `id`          | `GUID`      | <ul><li>Must not be empty GUID</li></ul>                                 | Unique identifier for this skin                                                                           |
| `directory`   | `string`    | <ul><li>Max 30 chars</li><li>`a–z` `A–Z` `0–9`</li><li>`-` `_`</li></ul> | Name of directory containing the skin files                                                               |
| `<pixelated>` | `boolean`   | <ul><li>`true` or `false`</li></ul>                                      | `true` disables smoothing (sharp pixels, useful for pixel art skins), `false` enables smoothing (default) |

## Example Manifest

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
                "directory": "CustomPeashooter"
            },
            {
                "type": "Wallnut",
                "name": "Gold Wallnut",
                "id": "00000000-0000-0000-0000-000000000000",
                "directory": "GoldWallnut"
            }
        ]
    }
}
```

- File structure:

```
📁 PVZ Replanted/
└── 📁 UserData/
    └── 📁 PvZRSkinPicker/
        └── 📁 SkinPacks/
            └── 📁 MySkinPack-V1/
                └── ...

```

`"directory": MySkin`

```
📁 MySkinPack-V1/
├── 🔢 manifest.json
└── 📁 MySkin/
    ├── 🖼️ skin.png
    ├── 📄 skin.atlas
    └── 🦴 skin.skel
```

Texture (`.png`), atlas (`.atlas`) & skeleton (`.skel`) files must be named `skin.{ext}`

## Plant types

| 1             | 2          | 3            | 4            | 5           | 6           | 7             |
| ------------- | ---------- | ------------ | ------------ | ----------- | ----------- | ------------- |
| Peashooter    | Sunflower  | Cherrybomb   | Wallnut      | Potatomine  | Snowpea     | Chomper       |
| Repeater      | Puffshroom | Sunshroom    | Fumeshroom   | Gravebuster | Hypnoshroom | Scaredyshroom |
| Iceshroom     | Doomshroom | Lilypad      | Squash       | Threepeater | Tanglekelp  | Jalapeno      |
| Spikeweed     | Torchwood  | Tallnut      | Seashroom    | Plantern    | Cactus      | Blover        |
| Splitpea      | Starfruit  | Pumpkinshell | Magnetshroom | Cabbagepult | Flowerpot   | Kernelpult    |
| InstantCoffee | Garlic     | Umbrella     | Marigold     | Melonpult   | Gatlingpea  | Twinsunflower |
| Gloomshroom   | Cattail    | Wintermelon  | GoldMagnet   | Spikerock   | Cobcannon   | Imitater      |

## Versioning

### Directory naming convention

Skin pack directories should use the `V-{N}` suffix to indicate the version:

```
📁 SkinPacks/
├── 📁 MySkinPack-V1/
│   ├── 🔢 manifest.json
│   └── ...
├── 📁 MySkinPack-V2/
│   ├── 🔢 manifest.json
│   └── ...
└── 📁 MySkinPack-V3/
    ├── 🔢 manifest.json
    └── ...
```

- Each version update should create a **new directory** with the updated version suffix (`V-1`, `V-2`, etc.)

- This prevents files from different versions from being accidentally combined when a newer version is unpacked over an existing installation

- The mod will automatically detect multiple versions of the same pack and only load the one with the **highest version number**

### Version header field

The `version` field in the `SkinPackHeader` should:

- Increment with each release
- Match the `V-{N}` suffix in the directory name

Example:
- Directory: `MySkinPack-V1/` → Header `"version": 1`
- Directory: `MySkinPack-V2/` → Header `"version": 2`

### GUID persistence

The `id` field in the `SkinPackHeader` and all `id` fields in `SkinEntry` objects are **permanent identifiers** that should never change after initial assignment:

- **Pack id:** Remains the same across all versions of your skin pack
- **Skin ids:** Should never change for the same skin, even across versions

This allows the application to track skin packs and skins, maintain user selections, and manage updates correctly

## Packaging

To simplify installation for users, skin packs should be distributed so the archive already contains the required folder structure

```
UserData/PvZRSkinPicker/SkinPacks
```

```
📁 UserData/
└── 📁 PvZRSkinPicker/
    └── 📁 SkinPacks/
        └── 📁 MySkinPack-V1/
            ├── 🔢 manifest.json
            └── ...
```

Packaging skin packs this way allows users to **extract the archive directly into the game directory**
