# Stage two visual continuity — 2026-11-03

User requested stage two be remade using stage one's map as the reference.
The previous smooth painted green crypt has been replaced with blue pixel-art
royal ruins consistent with SunlessForest.png. Four stacked panoramas cover
outer forest fortress, chain cloister, royal seal hall and Morga's crypt.

## Assets and integration

- `Assets/Sunfall/Resources/World/Stage2PixelRoomsV2.png`: 1086x1448 RGB,
  four equal 1086x362 strips. Same aspect-preserving parallax as before.
- `Assets/Sunfall/Resources/World/Stage2PixelSealV2.png`: 1536x1024 RGBA,
  closed/open cocoon repainted in coarse pixel clusters. Alpha verified.
- `RuinPlatform.png`: reuse the actual stage-one moss/stone asset for all
  stage-two floors and platforms. Source x16..2156, y250..556; same -10px
  grass offset, repeated at fixed aspect ratio with cropped last tile.
- Slight blue tint replaces green grading; no thin flat collision-edge stripe.

The seal atlas's actual separator is x748, not x768. Independent source regions
and ground anchors (384,978) / (1152,978) prevent neighboring fragments being
drawn or a shifted plinth during the transition. Both use identical scale.
Original V1 assets are retained but no longer loaded by stage two.

Combat layouts, enemy placement, Morga's design and attacks, chain-break timing,
dialogue, saved progress and rewards are unchanged. This is an art replacement.

## Validation

Core, Runtime, Editor, Tests all compile successfully.
Log: `Temp/Stage2PixelStyleCompile/compile.log`.
Images and alpha/source bounds inspected; no Play mode, QA or tests executed.

## Generation

Both images made with built-in image_gen; outputs copied into project without
image manipulation. Original outputs retained under Codex generated_images.

Background prompt (SunlessForest.png as style reference):

Use case: stylized-concept. Asset type: SUNFALL stage two side-scrolling game background atlas. The input image SunlessForest.png is the EXACT PIXEL ART STYLE reference: match crisp visible square pixel clusters, limited midnight blue / muted turquoise palette, moss olive highlights, blue atmospheric distance, tiny warm amber lamps, layered ruined gothic masonry. Create NEW locations in that same game, not a painterly illustration. Output portrait 1536x2048 with FOUR equal horizontal panoramic strips, each 1536x512, flush stacked at y0,512,1024,1536 with no borders or gaps. Each strip independent scene, all straight-on side scrolling camera, no perspective walkway or floor receding to a vanishing point. Image fills each strip. No characters, monsters, HUD, letters, foreground walkable platforms, big black foreground silhouettes or bloom. Lower 20% subdued dark blue fog for separately rendered gameplay. Distinct four rooms: TOP: outer ruined royal fortress reached from the forest, ivy-covered broken blue stone arches, distant woodland mist visible through arches, a few old iron bars. SECOND: chain prison cloister, repeated ruined arches and barred alcoves, heavy dark iron chains hanging in background recesses, roots and ferns reclaiming masonry. THIRD: inner royal seal hall, broad broken archways, dark carved sun insignia high on back wall, chipped stone buttresses, sparse faded crimson banners, cool mist. FOURTH: large chain warden boss crypt, tall ruined vaulted arches and broken roof opening toward blue fog, darker broad EMPTY middle area reserved for boss and separately rendered cocoon, warm small lamps confined to sides, no creature or cocoon in image. Progress from forest-lit blue to deeper blue sanctuary, avoid sickly yellow-green lighting. Pixel art must visibly use pixel stair-step outlines and clustered shading, not clean ink line art, smooth gradients, oil paint, photorealism or generic cartoon. Detailed but quiet background, all four strips consistent with supplied stage-one art.

Seal prompt (old seal as edit target, SunlessForest.png as style reference):

Use case: style-transfer. Image1 Stage2BrokenSealV1.png is the EDIT TARGET and exact closed/open cocoon design reference. Image2 SunlessForest.png is PIXEL ART STYLE reference only. Repaint image1 as crisp hand-crafted pixel art matching image2, coarse visible pixel clusters and stair-step edges rather than realistic brush texture. Keep two states in exactly two equal 768x1024 cells side by side on a 1536x1024 transparent atlas. LEFT closed ancient ivory shell cocoon wrapped by dark roots and bronze royal lock emblem, on ruined blue stone gothic plinth. RIGHT same plinth and SAME outer shell scale now split open to left and right, hollow empty dark cavity, broken lock fallen at foot. No creature. Keep all content within each cell with at least 30px side padding, top margin 50px, shared bottom stone baseline y950. Each plinth centered x384 and1152. Roots must stay within cell, no overlap. Match stage one's midnight blue stone, cool desaturated ivory plates, muted moss, restrained aged gold. Strong readable sprite silhouettes, less fine detail, approximate logical sprite resolution 192x256 per cell with nearest-neighbor-like pixel blocks. No gradients, painterly rendering, white glow or rectangular backdrop. Real transparent background and transparent open spaces around roots. No text or labels. Preserve a quiet dark central recess for a separately drawn boss. Do not include forest background.
