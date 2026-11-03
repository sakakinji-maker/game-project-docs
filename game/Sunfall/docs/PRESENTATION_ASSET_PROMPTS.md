# Presentation assets — 2026-11-02

Generated with the built-in image_gen tool. Alpha preserved; PNGs copied without raster postprocessing. Cell UVs are selected in code.

## Player spell effects

Final: `Assets/Sunfall/Resources/Effects/PlayerSpellsV2.png` (1254×1254, RGBA). V1 is retained as the initial generation.

Create one production game VFX sprite atlas for SUNFALL, dark gothic pixel-art action game. Transparent background, no panels, no icons borders, no text, no characters. EXACT 4 columns by 4 rows equal square cells, landscape square 1024x1024 canvas. Each effect isolated within its cell with 24 pixels transparent margin. Right facing projectiles. Row1: orange ember fireball with flame tail; long orange flaming spear; circular crimson flame explosion ring hollow center; horizontal gold iron hooked chain. Row2: vertical falling gold chain with pointed end; long gold sunlight spear; circular radiant golden sunburst hollow center; red heart shaped shattering blast. Row3: crimson spiked gear; violet crystal shard projectile; circular violet mirror shield hollow center; blue crescent blade. Row4: circular blue shadow veil hollow center; elaborate long white gold royal light lance; golden solar crown aura ring hollow center; small orange magical casting spark. Crisp detailed pixel clusters with controlled soft emissive edges, black/gold gothic motifs. Effects only with genuine alpha, no filled rectangular backdrops. Equal cell centers x128 384 640 896 and y128 384 640 896.

Layout correction (V1 used as edit reference):

Technical atlas layout correction ONLY. Preserve the SAME sixteen VFX in the same row-major order, colors and transparency. Repack into EXACT 4 equal columns x 4 equal rows. Every single effect must be SMALL and entirely inside its cell, including faint glow, with minimum 20% transparent padding on each side. Nothing crosses boundaries. The long spears must be SHORTENED to fit within the central 60% of their own cells, not reach adjacent cells. Especially row2 col2 sunlight spear must not touch row2col1 chain or row2col3 ring. Put every effect centered at exact cell center. All backgrounds truly transparent. No labels. Square canvas. No other changes.

## Equipment

Final: `Assets/Sunfall/Resources/Equipment/GothicEquipmentV1.png` (1536×1024, RGBA).

Create one production equipment inventory sprite atlas for SUNFALL dark gothic pixel-art game. Transparent background. EXACT 3 columns by 2 rows in a 1536x1024 canvas, equal 512x512 cells. Five isolated centered objects, entire object within central 380x380 each cell, broad transparent gutters; bottom right cell empty. Reading order: 1 elegant silver ash sword with black ornate guard and ember red gems, diagonal bottom-left to top-right. 2 furnace heart relic, burning crimson crystal heart inside tarnished gold iron cage. 3 sanctum seal, round ancient gold medallion with teal jewel and engraved solar rays. 4 pilgrim coat, charcoal long traveling coat with cream lining, gold clasp and burgundy trim. 5 pair of black raven boots with dark crimson feather decorations. Detailed crisp pixel art, cohesive limited palette, large readable silhouettes, rim highlights. No letters, no labels, no background, no shadows outside objects, no UI frames.
