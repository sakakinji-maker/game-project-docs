# 2스테이지 배경 해상도 보정 · 2026-11-03

본인 비교 화면에서 2층 배경이 1층보다 확대되어 보이는 문제를 확인했습니다.
1층 원본은 2172×724, 이전 2층 아틀라스는 1086×1448로 방 하나는 1086×362였습니다.
이 작은 조각을 1920×640으로 확대해 약 1.77배 크기의 픽셀이 표시되었습니다.

## 적용

- 방마다 독립 PNG 2172×724를 새로 생성했습니다. 단순 업스케일이 아닙니다.
- 전투방 순서: `Stage2GateHDV3`, `Stage2CloisterHDV3`, `Stage2SealHallHDV3`.
- 보스방: `Stage2BossCryptHDV3`.
- 모두 `Assets/Sunfall/Resources/World/`에 저장했습니다.
- 1층과 동일하게 폭 2070, 비율 유지 높이 690, 세로 기준 -105, 카메라 Y 보정 ×8을 사용합니다.
- 배경 필터 Point, 압축 없음, mipmap 없음, maxTextureSize 4096입니다.
- 이전 `Stage2PixelRoomsV2`는 보존하지만 런타임 배경으로 로드하지 않습니다.
- 봉인 도트, 모르가 디자인, 발판 좌표, 몬스터 배치, 연출 타이밍은 유지합니다.

## 확인

4개 PNG 실제 크기 2172×724 확인, 생성 이미지 시각 확인, Resources 연결 및 컴파일 확인.
`Temp/Stage2HDCompile/compile.log`: Core / Runtime / Editor / Tests 모두 exit=0.
Play·QA·테스트 실행·플레이어 빌드는 수행하지 않았습니다.

## 생성

내장 image_gen 사용. 참조는 기존 `World/SunlessForest.png` 한 장이며 생성 원본은 Codex generated_images에 보존했습니다.

### Stage2GateHDV3

Use case: stylized-concept. Create ONE full resolution panoramic background for the 2D side scrolling game SUNFALL. Output WIDE 3:1 panorama at 3072x1024, at least 2172x724. NOT a sheet, NOT multiple panels, NOT a cropped strip. Reference SunlessForest.png is the exact pixel-art quality and pixel density target: detailed hand-placed crisp pixel clusters, square-edged stone and leaves, muted blue atmospheric layers, readable masonry. Reimagine the SECOND STAGE ROOM 1: an ivy-covered ruined royal fortress gate at the edge of this forest. Left ancient dark tree and crumbled pointed arch, middle huge ruined blue-stone gatehouse and wrought iron barred arch, right a long broken royal wall and towers; distant forest silhouettes and layered aqueduct ruins fade into cool blue mist. Sparse old gold sun insignia and warm amber lanterns. Gothic blue-gray stone, dark turquoise vegetation, muted olive moss. Camera strictly side-on, medium architectural scale similar to reference. Foreground action space along lower 20% stays subdued and unobstructed, no walkable floors or floating platforms painted in. Beautiful varied masonry and fine pixel foliage, no blurry interpolation, no chunky enlargement, no painterly line art or smooth gradients. No characters, enemies, UI, text, border or watermark. Entire image one coherent room panorama. Match reference detail quality at native resolution.

### Stage2CloisterHDV3

Use case: stylized-concept. SUNFALL stage two room TWO chain prison cloister. ONE WIDE 3:1 panorama, 3072x1024 target, minimum2172x724. Never multiple panels or sprite atlas. Input SunlessForest.png defines exact pixel-art density, material quality and blue atmospheric palette. Side-on 2D action game background, tall but broken gothic arcade stretching laterally across scene, close weathered blue stone columns on left and right, several arched prison alcoves with iron bars, substantial old iron chains hanging in curved arcs between wall anchors, patches of ivy and ferns, small distant arches visible through collapsed wall sections, layered fog and forest towers beyond. Tiny warm amber lamps; restrained gold royal details. Higher enclosed stone ceiling than forest but same world and pixel texture. Finely clustered stone/leaf pixels of reference, no blurry enlargement, no smooth cartoon ink lines, no photorealism. Lower20percent quiet dark blue fog, no foreground gameplay platforms or obstacles. Entire width one continuous coherent room, no centered perspective tunnel, no characters, no boss, no text, labels, UI, border or watermark. Balanced detail and depth, cool navy and muted teal, not green-yellow.

### Stage2SealHallHDV3

Use case: stylized-concept. SUNFALL stage two room THREE royal seal hall. ONE full resolution 3:1 panoramic background at 3072x1024 target, minimum2172x724, not a sheet or panels. Exact stylistic reference SunlessForest.png: dense crisp square pixel clusters, atmospheric depth, blue stone gothic ruins and detailed ivy, small warm amber lights, no blurry enlargement. Side-view 2D action game background. Long ruined royal hall, massive chipped blue masonry arches framing an ancient back wall, one large weathered carved sun seal high on the central wall, subdued dark crimson torn banners, cracked columns, ivy creeping around old royal carvings, gaps in the roof and walls reveal distant aqueducts and blue forest mist. Keep architecture medium scale similar to the stage-one reference, wide lateral composition instead of perspective tunnel. A quiet foggy lower 20% reserved for gameplay, no painted walkable ledges or floating platforms. Dense fine pixel detail across the entire canvas, no smooth brush painting, no cartoon outlines, no soft blur, no modern objects. Royal seal theme but not a bright church. No people, monsters, cocoon, text, labels, HUD, watermark or borders. Full width single coherent environment, detailed navy/teal stone, muted moss and gold.

### Stage2BossCryptHDV3

Use case: stylized-concept. SUNFALL stage two BOSS ROOM royal chain crypt. ONE independent wide panoramic background, aspect3:1 target3072x1024 minimum2172x724. Reference SunlessForest.png exact detailed pixel-art treatment and pixel density: fine crisp clustered pixels, medieval blue masonry, foliage depth and muted blue mist. A huge ruined gothic crypt seen SIDE-ON for a 2D action game. Tall cracked arches at far left and right, hanging old black iron chains attached to stone wall anchors, restrained royal gold motifs, tiny warm amber lamps near edges. Broad quiet open CENTER with blue mist and distant wall niches, enough dark negative space for a separately rendered ivory cocoon and boss. Partial ruined ceiling reveals deep blue night and far fortress silhouettes. Foreground action space lower20percent subdued and empty; NO painted altar, cocoon, throne, monster or playable platform. Detailed full-width stonework at the same visual scale as stage-one reference, moss/roots confined to architecture, limited midnightblue mutedturquoise olive gold. No green fluorescent lighting, photorealism, painterly smoothness, cartoon outlines, blur or oversize pixel blocks. NO tiled panels, sheets, labels, letters, UI, borders or watermark. One panoramic boss environment with grand framing and readable central combat space.

