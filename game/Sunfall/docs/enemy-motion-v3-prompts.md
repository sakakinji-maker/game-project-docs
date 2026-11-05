# Enemy motion V3 — built-in image generation

Saved assets: `Sunfall/Assets/Sunfall/Resources/Demons/Motion/` (10 RGBA sheets, 24 poses each).

Stage7 correction prompt (built-in edit, using the generated Stage7 sheet):
Precise sprite atlas edit. Preserve this EXACT 1024x1536 RGBA transparent 4-column 6-row grid of 24 frames, keep ALL body sizes, identities, individual poses, outfits and positions exactly. Fix ONLY the sword in fourth row (frames12,13,14,15): at frame13 (fourth row second cell) and frame14 (fourth row third cell) angle the blade diagonally downward/right slightly so its tip stays at least 12 pixels inside its OWN 256x256 cell. Remove stray blade pixels from left edge of fourth row fourth cell, preserve that character entirely. No other change. Keep uniform body scales and current transparent background. Do not shrink entire character. No borders.

Use case: stylized-concept. Asset type: production animation sprite sheet for existing 2D gothic action game. Input is identity/style reference, keep EXACT character identity, costume, weapon, proportions and palette. Re-draw continuous actual intermediate poses, not duplicates or transparency crossfades. Create ONE transparent 1024x1536 portrait atlas, EXACTLY 4 columns x 6 rows of equal square cells, exactly 24 sprites in strict uniform grid. No cell lines or text, no shadows or scenery, no labels. Right-facing three-quarter side view in every cell. Every full body and all weapons/wings remain inside its cell with at least 10% transparent gutters. Consistent camera and body scale ALL frames, feet/virtual flight base on y=85% of cell, pelvis aligned at x=50% cell, head height consistent (body crown-to-feet approximately 65% cell); wings and raised weapons may extend above head, do NOT shrink bodies to fit attack. Crisp detailed pixel-art clusters matching reference, opaque sprite interiors, true transparent empty canvas. Reading order left-right top-bottom: frames 0..7 (first TWO rows) one smooth complete eight-frame locomotion cycle; frames 8..11 (third row) four successive anticipation/windup poses for primary attack, frames 12..15 (fourth row) contact, follow-through, settle, neutral recovery; frames 16..19 (fifth row) four successive magic/special anticipation poses; frames 20..23 (sixth row) release, follow-through, settle, neutral recovery. Do not draw large attached effects, those are rendered separately by the game. No extra limbs, no pose touching cell boundary.

MeleeMotionV3
Reference: Demons/Melee.png
Character: hooded female bat-winged demon melee soldier, dark armor and burgundy cloth, short poleaxe. Locomotion: grounded deliberate walking. Primary attack: two-handed diagonal axe chop. Special: free-hand curse gesture, weapon held aside.

ArcherMotionV3
Reference: Demons/Archer.png
Character: hooded female bat-winged demon archer, dark armor, purple cloth, bow and quiver. Locomotion: grounded cautious walking holding bow. Primary attack: draw bow and release arrow, arrow should disappear after release. Special: raise bow for high aimed shot and return.

HeavyMotionV3
Reference: Demons/Heavy.png
Character: heavy black-and-gold plate demon soldier with red bat wings and great axe. Locomotion: heavy grounded marching steps. Primary attack: heavy two-handed axe overhead strike. Special: plant axe and brace armored shoulders.

Stage1MotionV3
Reference: Demons/Stage1BossFlight.png
Character: adult black-haired red-winged gothic vampire woman, white blouse, black corset and burgundy cape. Locomotion: hovering flight, smooth complete bat-wing upstroke/downstroke loop. Primary attack: lean into a forward flying lunge then recover upright. Special: raise hand and release red magic then lower hand; no detached projectile.

Stage2MotionV3
Reference: Demons/Stage2MorgaHuskV2.png
Character: Morga: large black root-and-stone husk with two ivory split shell plates, exposed white skeletal face and rib roots, shackles on wrists. Locomotion: lumbering rooted steps with natural alternating arms. Primary attack: raise both heavy arms then slam downward and recover. Special: extend shackled hand and summon binding magic then retract.

Stage3MotionV3
Reference: Demons/Stage3AurelEclipseV2.png
Character: Aurel: black faceless hood, amber eyes, ornate gold broken solar halo, dark feather mantle, ivory shoulder cloth, ceremonial crescent blade. Locomotion: slow regal grounded walking, trailing cloth. Primary attack: draw ceremonial crescent blade back then sweep across and recover. Special: raise free hand palm upward, gather sun light then extend hand and lower.

Stage4MotionV3
Reference: Demons/Stage4VarkanHammerV2.png
Character: Varkan: massive dark armored knight, skull helm, red chest gem, gold chain trim, torn charcoal cape, heavy two-handed war hammer. Locomotion: weighty armored walking while carrying hammer. Primary attack: lift hammer through four anticipation poses then slam and recover through four poses. Special: plant hammer beside body and raise free gauntlet for royal magic.

Stage5MotionV3
Reference: Demons/Stage5LysseaMaskBodyV2.png
Character: Lyssea: adult elegant faceless female mirror priestess, dark burgundy layered dress, silver thorn collar, pale hands, flowing dark hair, blank black oval face reserved for separate mask overlay. Locomotion: slow gliding walk, natural sleeve and dress motion. Primary attack: crouch and gather sleeves then thrust one hand forward and recover. Special: raise both open hands, spread wide to cast mirror magic, then recover; keep face blank.

Stage6MotionV3
Reference: Demons/Stage6NoctisRavenV2.png
Character: Noctis: adult gothic raven-winged woman, long black-silver hair, black and red dress, red-black ornate scythe. Locomotion: hovering flight with eight distinct sequential feathered wing positions, body root stable. Primary attack: raise scythe then sweeping airborne slash and recovery. Special: raise free hand while scythe held beside body, summon night magic, lower hand.

Stage7MotionV3
Reference: Demons/Stage7SolveinThornV2.png
Character: Solvein: tall faceless crowned king in thorn-like dark plate, red glowing eyes, torn black-burgundy cape, long crimson rune greatsword. Locomotion: slow heavy kingly walking with sword held steady. Primary attack: raise greatsword then broad forward cleave and recover. Special: lower sword one-handed, lift empty hand and release royal sun magic then recover.
