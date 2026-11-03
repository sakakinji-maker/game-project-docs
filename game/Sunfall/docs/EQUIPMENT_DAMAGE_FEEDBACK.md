# Equipment damage inspection and feedback

Updated 2026-11-03

## Offensive equipment balance update (2026-11-03)

At the user's request, offensive equipment now gives a larger damage increase:

| Item | Base bonus (old → new) | Per upgrade (old → new) | +5 total (old → new) |
| --- | --- | --- | --- |
| Ash Sword | +10% → +30% sword damage | +2 → +8 percentage points | +20% → +70% |
| Furnace Heart | +12% → +25% magic damage | +2 → +7 percentage points | +22% → +60% |

With no growth points, sword hits now deal 31.2 at +0, 33.12 at +1,
and 40.8 at +5. The unupgraded sword's three-hit combo totals 112.32,
enough for a first-stage 100 HP heavy enemy if all hits connect.
Ember Wave deals 75 with the unupgraded heart and 96 with a +5 heart.

Descriptions, upgrade previews and combat multipliers share the same damage
bonus function. Existing owned/equipped items and saved upgrades automatically
use the new values; no save reset or migration is required. Costs, +5 cap and
non-damage equipment effects are unchanged.

Existing damage regression cases were updated, including +1 and +5 variants.
Compile log: `Sunfall/Temp/EquipmentBonusCompile/compile.log`.
Core, Runtime, Editor and Tests compiled only; no Play mode or tests executed.

The sections below record the earlier inspection, before this balance change.

## Findings from code inspection

Equipment multipliers are applied by `SwordDamage` / `MagicDamage`. Ground
combo hits, aerial slashes, projectiles, area spells and late-stage skills use
these values. Village practice and new adventures share the equipped inventory.
No missing equipment multiplier was found in these paths.

The unupgraded Ash Sword changes a basic hit from 24 to 26.4 (+10%).
A first-stage melee enemy has 44 HP, so both values still require two basic
hits. A heavy enemy has 100 HP: both the base combo (86.4 total) and equipped
combo (95.04 total) still need another hit. This can conceal the improvement.
Furnace Heart increases magic only; armor, boots and the cooldown relic do not
increase sword damage. Buying or looting an item does not automatically equip it.

## Changes

- Inventory shows equipment's contribution separately and reports before/after
  sword and magic values when an item is equipped, replaced or removed.
- Inventory and growth screens read the same model properties as combat.
- Enemy hit numbers display resolved strike damage, including overkill:
  gold for sword, violet for magic. Values retain one decimal when needed.
- Feedback is anchored in world coordinates, fades after 0.75 simulation
  seconds, is capped at 64 entries and clears on room changes. It does not
  add flashes, camera shake or hit pauses.
- Equipment balance, costs, upgrades and save format remain unchanged.

## Validation

Core, Runtime, Editor and Tests compiled successfully using
`tools/Compile-AttackArt.ps1 -OutputFolder Temp/EquipmentDamageFixCompile`.
Log: `Sunfall/Temp/EquipmentDamageFixCompile/compile.log`.

Regression cases were added for ground/air hits at +0 and +5, combo finishers,
magic damage with/without the relic, feedback expiry, unequip and new-run
equipment retention. Tests were compiled only, not executed. Unity Play mode
and gameplay QA were not performed, as requested by the user.
