# Forest decoration replacement — 2026-11-03

Replaced the screenshot's stage-one rectangle-based scenery with six generated
pixel-art props: ruined arch, wooden watchtower, sun medallion, masonry rubble,
broken planter and supply crate. Stage-one room decorations and boss-arena
decorations use the atlas. Shared arch callers in chapters three and four also
use the new image. Other chapters' bespoke architecture was not redesigned.

Asset: `Assets/Sunfall/Resources/World/ForestRuinPropsV1.png` (1536 × 1024 RGBA).
Created with the built-in image_gen tool; the supplied screenshot was a style
reference. Original generated output retained in the Codex generated_images folder.
The atlas was copied without image manipulation. Alpha samples were checked:
background / arch opening alpha 0; visible stone alpha around 251.

The generator did not follow equal-height cells. Runtime source rectangles
therefore use measured individual regions and visible ground anchors rather
than uniform grid slices. Aspect ratio is preserved, point sampling is enabled,
and the existing WorldArtImporter disables mipmaps and texture compression.
Missing decorative textures retry loading and do not draw primitive boxes.

Boss-arena hanging chains and drifting embers now reuse existing image-based
chain/particle rendering. Stage-one collision-edge debug-like stripes were
removed visually. Gameplay platform positions, collision and enemies are unchanged.

Validation: Core, Runtime, Editor and Tests compiled successfully.
Log: `Temp/ForestPropsCompile/compile.log`. No Unity Play mode or tests executed.

## Generation prompt

Use case: stylized-concept. Create a production-ready TRANSPARENT pixel-art sprite atlas for SUNFALL, a dark gothic side-scrolling action game. Attached screenshot is STYLE REFERENCE ONLY: match its muted blue-teal stone, pale olive moss, crisp pixel clusters and straight-on 2D side-view. Do not recreate screenshot or characters. Exactly six isolated props in a rigid 3 columns x 2 rows equal-cell grid, landscape 1536x1024. Each prop entirely inside its cell with 32px empty transparent margin, nothing crosses cell boundaries. Top row left: a tall broken pointed gothic stone arch, two complete footed pillars, missing keystone, thin ivy, transparent open center. Top row middle: decrepit tall wooden watchtower with diagonal braces and small tattered crimson banner, no ladder/platform implying playable foreground, transparent gaps. Top row right: round weathered sun-seal stone medallion with broken rim and carved gold runes, no text. Bottom row left: low asymmetrical pile of cracked blue-gray masonry rubble with moss and small fern. Bottom row middle: low broken gothic stone planter filled with dry tangled stems and sparse moss. Bottom row right: low battered dark wooden supply crate, iron bands and a little crimson cloth, chipped irregular silhouette. All same rendering style, hand-crafted crisp pixel art approximately 128px logical detail per cell, limited cool palette, subtle highlights, detailed material surfaces not flat rectangles. Objects grounded, no cast shadow outside silhouette. No scenery, floor, ground plane, people, labels, letters, border, frame, watermark, checkerboard, glow fog or opaque backing; true transparent alpha around and inside structures.
