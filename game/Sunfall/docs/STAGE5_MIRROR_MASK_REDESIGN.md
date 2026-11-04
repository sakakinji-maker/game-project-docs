# 5스테이지 거울 성역 — 탈과 유리 파손 연출

2026-11-04. Built-in image_gen 사용. 게임 플레이 및 QA는 실행하지 않음.

## 반영 범위
- 거울 정원 → 기억의 회랑 → 뒤집힌 예배당 → 거울지기의 성역, 각 방 2172×724 배경.
- 붉은 스테인드글라스와 검은 은장식 거울, 와인색/차가운 석조 배색. 기존 고딕 석재 발판 아트 재사용.
- 리세아: 검은 머리, 와인색 예복, 은 장식, 전통 탈에서 착안한 세 가지 가면. 몸 8프레임과 탈을 분리해 패턴별 교체.
- 웃는 탈 = 거울 도약, 성난 탈 = 거울 파편 탄막, 우는 탈 = 거울 감옥. 패턴 확정 시 Windup부터 해당 탈 표시, 다음 패턴까지 유지.
- 등장: 0–0.5초 온전한 거울, 0.5–1.15초 균열/얼굴, 1.15초 파손, 1.15–1.95초 보스 출현, 2.45초까지 파편 소멸, 2.5초부터 등장 대사. 카메라 흔들림 없음.
- 파손된 프레임은 전투 후에도 남으며 처치 시 기존 금빛 봉합 연출 연결.
- 6종 대화 초상화와 탈 관련 대사, 경고 문구 반영.
- Core 전투 판정/공격력/HP/배치/진행/보상/스킬/저장 데이터 변경 없음.

## 파일
모든 리소스는 `Sunfall/Assets/Sunfall/Resources/` 아래.

| 파일 | 용도 |
|---|---|
| World/Stage5MirrorGardenHDV2.png | 1번 방 |
| World/Stage5MemoryHallHDV2.png | 2번 방 |
| World/Stage5InvertedChapelHDV2.png | 3번 방 |
| World/Stage5MirrorSanctumHDV2.png | 보스방 |
| World/Stage5MasksMirrorV2.png | 1536×1024, 윗줄 웃음/분노/눈물 탈, 아랫줄 거울 온전/균열/파손 |
| Demons/Stage5LysseaMaskBodyV2.png | 1774×887, 4×2 몸 애니메이션 |
| Story/BossPortraits/LysseaMaskExpressionsV2.png | 1536×1024, 3×2 대화 표정 |

주요 코드: Runtime/SunfallGame.MirrorSanctum.cs, LateBosses.cs, Map.cs, BossIntro.cs, Story.cs, LateStory.cs, BossPortraits.cs, PresentationArt.cs. 인게임 몸 스케일은 모든 자세에 0.4 고정. 얼굴 앵커만 자세별 정렬. PNG를 가공하거나 리사이즈하지 않고 생성된 알파를 보존하여 복사. 배경 무압축/밉맵 없음/NPOT 유지/최대4096, 몸 최대2048.

## 확인
`tools/Compile-AttackArt.ps1 -OutputFolder Temp/Stage5MirrorCompile`로 Core/Runtime/Editor/Tests 소스 컴파일. 테스트 실행이나 Unity Play는 하지 않음. 로그: `Sunfall/Temp/Stage5MirrorCompile/compile.log`.

에셋 원본 크기·알파·셀 경계와 코드 연결을 정적으로 확인. 실제 화면의 겹침, 전투 가독성, 연출 리듬은 사용자 QA 대상.

## 생성 프롬프트 원문

### garden

ONE ultra-wide3:1 3072x1024 detailed crisp pixel-art panorama for SUNFALL stage5 mirror garden. Reference1 red stainedglass gothic mood; reference2 existing game pixel density and royal fortress continuity. First room beyond stage4royal fortress: abandoned enclosed cloistergarden, black bare trees, cracked silver ornate standing mirrors among dead roses, shallow dark reflectingpool, stonearcades and distant royal roofs. Tall red stainedglass windows in rearwall emit stored crimson memorylight (no actual sunlight), their red reflections cross stillwater, small amber lamps, cold slate shadows. SIDE-ON horizontal 2D platformer composition, no vanishing perspective road, no foreground trees hiding actors. Bottom20% quiet dark foundation behind separately rendered platforms. Fine deliberate pixelclusters same density asreference2, richly chipped masonry, red/charcoal/dustysilver palette. No characters/UI/text/logos or floatingplayableplatforms, one continuous fullresolution panorama no panels.

### corridor

ONE ultra-wide3:1 3072x1024 crisp highdetail pixel-art panorama SUNFALL stage5 memory corridor. Ref1 tall gothic red stainedglass, rubylight shafts casting patterns on dark floor. Ref2 tarnished ornate black silver standing mirror, old charcoal woodpaneling, deep wine rug. Ref3 exactexistinggame pixel density. Reimagine as SIDE-ON 2D side-scroll long horizontal hall, repeated stonecolumns and redglass lancets alternate with tall antique mirrors, mirror reflections subtly show warm small home door and faint lamplit table, no people. Dark stone, blackened carvedwood, dustysilver mirrors, burgundy rugs, small warmcandle sconces. Broad redlight patterns in middle floor, quiet bottom20% shadowed foundations behind gameplay. No foregroundcolumns blockingactors, no floating platforms, no perspective tunnel, no labels/UI/logos. Fine sharp deliberatepixelclusters, not photoreal/blur, one continuous panoramicroom notcollage.

### chapel

ONE ultra-wide3:1 3072x1024 detailed pixel-art background SUNFALL stage5 inverted chapel, side-scrolling gothic fantasy. Reference1 crimson stainedglass shafts and blackstone; reference2 old ornate mirrors; reference3 sharp gamepixel density. Long SIDE-ON chapel wall with three enormous tarnished mirror panels; INSIDE reflections only, gothic rib vaults and chandeliers appear upside down, strange impossible reflected arches. Actual foreground/midground floor remains straight normal stone with wine runners, no reversed playablefloor. Redstainedglass windows alternating with mirrors, chipped charcoal stone columns, faint silver-blue glass edges, sparsegoldcandlelight. Unsettling dream atmosphere yet readable finepixels, not modern abstract kaleidoscope. Quiet bottom20% foundation, no foreground blockers, characters, floating platforms, HUD, text, logos or collage. Single panoramic room crisp tiny pixelclusters, no blur.

### sanctum

ONE ultra-wide3:1 3072x1024 sharp detailed pixel-art background SUNFALL stage5 boss mirror sanctuary. Ref1 old ornate blackmirror room mood and weathered blackwood panels, tarnishedsilver carving, faded winecarpet; Ref2 red stainedglass shafts; Ref3 gamepixel density. Wide SIDE-ON gothic manorhall, long wall with side mirrors reflecting ghostly chapelwindows, candlesconces, crackedplaster, towering rubyglass atleft/right. Very center a dark EMPTY recessed arch, widthabout15%image reserved for SEPARATELY animated standingmirror, so do NOT paint centralmirror orboss. Calm broad darkstonefloor bottomquarter, burgundy rug, no obstructingforeground pillars or furniture. Elegant decrepit mood, black/silver/deepcrimson with small warmcandlelights. One continuous 2D platformer panoramic background, no overhead/isometric perspective, no characters/UI/text/logo/collage/playableplatforms. Fine crisp pixel clusters no photorealblur.

### body

Production transparent pixelart sprite sheet of original gothic mirror demon LYSSEA, elegant adult female masked theatrical spirit, SUNFALL 2D game. Ref is traditional carved tal mask aesthetic for silhouette/mood ONLY, face mask will be added separately in game. Draw body with featureless DARK BLACK OVAL FACE recess framed by blackhair and silverheadband, NO eyes/nose/mouth/mask onface. Face must remain exposed unobstructed byhands. Outfit black and deepwine layered brocade robe, long split skirt over boots, wide flowing sleeves with ivorysilver lining, antiqued mirror shards as collar, blackhair ribbons, NO crown or large horns. Transparent atlas exact4columns2rows wide2:1 target2048x1024, eight same-scale fullbody poses, footbaseline90%cell, bodycenter50%, helmet/headsizeconsistent, generous15pxmargins. Right-facing slight3/4 FRONT orientation so faceoval readable. Row1 idle1, idle2sleevesdrift, glidestep1, glidestep2. Row2 teleportwindup crouch armsdrawnin, teleportrelease leaningforward armextended, glassfan cast onehandextended, prisoncast botharmsraised. Same face orientation mostlyupright for removablemask attachment, eachfaceoval approximately50x65px at1774x887 output. Crisp detailedpixelclusters, readable grownup proportions, no scenery, floor shadow, text, labels, grid or background. Entirebodyhairandhandsinsideeachcell. Do not copy carved face into body; truly blank black face is essential for mask layering.

### bodyPadding

EDIT this exact Lyssea 4x2 sprite atlas. Preserve all eight poses, outfit, blank blackfaceoval, orientation and style. Repair top-edge clipped hair and bottom-edge robe/glow fragments: uniformly reduce ALL eight characters to85% size inside their cells, whole figure fully inside with minimum25px transparent margin on EVERY cell edge. Place feet/robe baseline at90% ofeachcell. Remove stray pixels outside silhouettes. Restore complete top hair ornaments, do not truncate. Exact4columns2rows wide2:1 canvas target2048x1024. Same armor/body/headsizes across allframes, no text/background/grid/shadow. Face slot remains black oval with NO features or mask; runtimewilladd separatewoodenmask. All sprites including flying hair and sleeves fully contained, generousclean transparent gutters.

### masksMirror

Create a production sprite atlas for a gothic pixel-art sidescroller, 1536x1024, EXACTLY 3 columns x2 rows equal512 square cells on TRUE TRANSPARENT background. References: image1 carved Korean theatrical tal masks, image2 antique gothic mirror. TOP ROW: three front-facing masks, same outer shape and size, centered at local(256,256), each fits local x130..382 y80..430. Mask0 laughing sly curved eyes and wide toothy smile; mask1 wrathful sharply angled eyebrows and snarling open mouth; mask2 weeping downturned eyebrows and long mournful mouth with carved tears. Beautiful pale aged ivory and dark carved wood shadows, small muted silver engraved edge, distinctly inspired by rounded traditional tal noses/brows, NOT generic Venetian masks. Crisp readable pixel art, chunky expressive eyes/mouth recognizable at 20 pixels. BOTTOM ROW: SAME tall ornate blackened silver standing mirror in three states: intact reflecting dim red gothic window; heavily cracked glass; shattered empty frame with only jagged glass around edges and TRANSPARENT empty middle. Mirror frame geometry identical in all3states, fits local x100..412 y30..485, feetbaseline485, no debris outsidecells. No text labels grids ground castshadows or extra objects. Accurate orthographic front view. Pixelated hand placed clusters, no photographic blur. Keep each asset isolated with transparent gutters.

### portraits

Production dialogue portrait atlas for gothic fantasy game, TRUE TRANSPARENT,1536x1024 exact3columns2rows512squarecells. Reference1 is Lyssea body identity: adult feminine ghost, long black hair, wine-black brocade robe, ivory sleeves,silver ornamental collar. Reference2 top row gives her three traditional carved tal masks: laughing,angry,crying. Create SIX distinct waist-up portraits of THIS same masked ghost, wearing these masks (no visible human face), centered same headscale, fully insideeachcell with32pxgutter, crown ofhairnevercropped, bust endsbottom. Row1: neutral with smiling mask,handsfolded; mocking laughing mask,headtilted,openpalm; furious angry mask,raisedclawedhand. Row2: shocked with cracked smiling mask,recoiling; pained crying mask,handtochest; resigned crying mask,headbowed,handslowered. Keep masks readable but naturally worn onface (not oversized floating props). Highquality illustrated anime/gothic portrait rendering, rich subtlefabric and clean silhouette, ivorymaskcarving recognizable, darkburgundy/silverpalette. Not photo. Transparent background no environment, no title no labels no grids no extraheads. Match identity and consistenthumanproportions.

