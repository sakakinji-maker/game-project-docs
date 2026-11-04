# 6스테이지 — 일식의 첨탑 / 녹티스 리디자인

2026-11-04. 사용자 참고 3장에 맞춰 built-in image_gen으로 제작. Unity Play 및 게임 QA 미실행.

## 구현
- 밤의 기단: 달빛 아래 가고일과 고딕 탑 진입로.
- 일식의 나선 계단: 창문과 다층 회랑이 있는 탑 내부.
- 부서진 종루: 깨진 종, 무너진 아치, 바람에 노출된 고층 갤러리.
- 일식의 원형 성소: 중앙 석상, 원형 열주 회랑, 열린 천장.
- 녹티스: 검은 깃털 날개, 검정/와인색 예복과 갑주, 은빛 머리끝, 붉은 낫날. 동일 몸 스케일 0.5, 8개 자세의 발 기준점 수동 정렬. 6프레임 순서로 비행 날갯짓.
- 등장: 어두운 실루엣에서 색이 드러나며 135 논리 픽셀 높이에서 하강. 0.4–1.85초 하강, 깃털 소용돌이 2.35초까지, 대사 2.5초 이후. 화면 흔들림 없음. 충돌 좌표 변경 없음.
- 기존 CrescentDive는 낫 궤적, MoonVolley는 진행 방향을 향한 깃털 칼날, NightVeil은 깃털/바람 장막으로 표현. 위험 영역에 GUI clipping을 적용하여 초록 안전지대로 이펙트가 번지지 않음.
- 중앙 일식 장식은 처치 후 사라지며 금빛 봉합 자국이 남음.
- 여섯 표정 초상화 및 낫/종루 설정을 반영한 등장·처치 대사.
- 기존 Core 전투 판정, 2페이즈, 몬스터/발판 배치, 획득 스킬, 보상, 저장 데이터, 7스테이지 진행은 변경하지 않음.

## 최종 이미지 위치
모두 `Sunfall/Assets/Sunfall/Resources/` 아래.

| 파일 | 크기 / 용도 |
|---|---|
| World/Stage6MoonBridgeHDV2.png | 2172×724 진입로 |
| World/Stage6SpiralGalleryHDV2.png | 2172×724 내부 회랑 |
| World/Stage6BrokenBelfryHDV2.png | 2172×724 종루 |
| World/Stage6EclipseRotundaHDV2.png | 2172×724 보스방 |
| Demons/Stage6NoctisRavenV2.png | 1774×887, 4×2 도트 자세 |
| Story/BossPortraits/NoctisRavenExpressionsV2.png | 1536×1024, 3×2 초상화 |
| Effects/Stage6RavenEclipseV2.png | 1254×1254, 2×2 깃털/낫 궤적/일식/깃털 고리 |

PNG 원본 크기와 알파를 유지해 복사했으며 이미지 스크립트 편집은 하지 않음. 초기 잘린 몸/초상화는 이미지 생성 도구로 여백을 수정한 버전을 사용. 배경은 무압축·밉맵 없음·NPOT 크기 유지·최대4096. 지형은 기존 고딕 석재 아트를 공유.

## 코드
- Runtime/SunfallGame.EclipseTower.cs: 배경, 일식, 깃털, 등장 및 공격 이펙트.
- Runtime/SunfallGame.LateBosses.cs: 새 도트/앵커/비행 루프/공격 및 등장 표시.
- Runtime/SunfallGame.Map.cs, LateMaps.cs: 6스테이지 전용 배경과 지형 연결.
- Runtime/SunfallGame.BossEffects.cs: 녹티스 탄막 분기.
- Runtime/SunfallGame.BossIntro.cs, Story.cs, LateStory.cs, BossPortraits.cs: 등장·대사·초상화.
- Editor/DemonArtImporter.cs: 새 보스 에셋 복구 목록.

## 검증
`tools/Compile-AttackArt.ps1 -OutputFolder Temp/Stage6EclipseCompile` 실행: Core/Runtime/Editor/Tests 모두 exit=0. Tests는 소스 컴파일만 했고 실행하지 않음.
로그: `Sunfall/Temp/Stage6EclipseCompile/compile.log`.
이미지 해상도/알파, 모든 몸·초상화 셀의 여백, 리소스 경로, GUID 중복 여부 정적 확인. 실제 게임 화면 가독성과 연출 타이밍은 사용자 QA 대상. F6은 기존 개발자 6스테이지 첫 방 이동 키.

## 생성 프롬프트 원문

### approach

Production background panorama for a high quality gothic PIXEL ART 2D side scrolling action game. Wide 3:1 canvas 3072x1024. Reference is mood/architecture only, recompose side on. Stage6 room1: night foundations of an immense moonlit gothic bell tower beyond a royal mirror sanctuary. Weathered grey-blue masonry bridges span an abyss of blue mist, seated stone gargoyles on parapets, distant needle spires, pale moon high left behind clouds. Clear depth layers, intricate hand-pixel clusters, sharp readable edges and restrained cyan rimlight, no photoreal blur. Background only: keep bottom quarter dark quiet stone/mist to sit behind separate playable platforms and characters, no giant foreground blockers, no humans enemies UI letters watermark. Rich dark teal and navy, silver moon highlights and very sparse dim amber windows. Detailed coherent architecture and atmospheric depth at same density as a premium pixel-art forest ruin game. Full single panorama, no panels or tiles.

### stairs

Create a single full panorama 3072x1024, 3:1, for Stage6 room2 of a detailed gothic pixel-art side scrolling game. Reference1 establishes moonlit teal gargoyle tower, reference2 establishes ornate pillared interior. Scene: inside a vast tall gothic bell tower, layered curved stair landings and narrow stone galleries at different background heights, pointed windows open to pale blue moonlit cloud sea, carved pillars, distant iron bell mechanisms above, weathered royal masonry and gargoyle brackets. Orthographic side-view game backdrop with depth in distant stairs; NOT looking down a tunnel. Clear tiny hand-crafted pixel clusters, no blurry photos or smooth painted smears. Navy/teal/stone silver palette, selective moonlight shafts through windows; bottom quarter dark restrained masonry behind separate playable platforms. Architecture occupies whole panorama, no characters enemies text UI watermarks or massive foreground pillars obscuring action. Match premium pixel-art ruin visual density. Single scene without panels, crisp details, warm lights almost absent.

### belfry

Single ultrawide 3:1 panorama3072x1024 for detailed gothic pixel art side-scrolling action game, Stage6 room3 broken belfry. Use reference moonlit gothic gargoyle tower and gloomy teal atmosphere. Side-on view across a high ruined bell gallery: cracked giant bronze bell suspended near upper left, another broken bell on a DISTANT platform, shattered arches open to navy storm clouds and cold blue moonlight, delicate ironwork, gargoyle corbels, weathered stone and traces of royal silver trim. One immense dark spire far behind, drifting cloud layers, windswept torn BLACK banners. Bottom quarter low contrast shadowed rear wall and haze with NO foreground ledges or walkway silhouettes that could be mistaken for playable platforms. Crisp hand drawn pixel clusters and rich stone carving, consistent premium pixel game density, no blur no photographs, no characters, no birds baked into foreground, no UI words grids watermarks. Moody cold blue with dim old bronze, detailed but readable. This is a distant decorative background behind separate terrain sprites.

### rotunda

3072x1024 3:1 single panorama, premium detailed PIXEL ART game background for final room of Stage6 moonlit eclipse tower. Reference1 round vaulted rotunda and statue architecture, reference2 exterior teal moonlit tower. Reinterpret as a ruined summit rotunda seen STRAIGHT SIDE ON for a 2D sidescroller: huge elegant central stone goddess statue on tall worn plinth in rear middle, one arm raised to broken circular roof oculus. Circular gallery of pale carved columns and gothic arches, subtly curved apse, shattered dome open to navy sky. Cold moonlight enters from above, dark grey-blue stone and silver trim, only faint desaturated rose rimlight near top center. NO actual moon/eclipse disk painted in center (it will be animated separately), leave dark open sky near top center of oculus. Statue occupies rear middle, subtle enough not to hide boss in foreground. Bottom quarter dark quiet cracked rear floor, no foreground railings/characters/figures/enemies/pillars obscuring action. Crisp small pixel clusters, coherent stone texture, no photographic grain or blur, no letters UI or watermark. Gothic royal tragedy atmosphere, not bright daylight. Whole wide room no panels.

### body

Create a production transparent sprite sheet for a gothic pixel-art sidescrolling boss Noctis, inspired by reference character but ORIGINAL design. Mature adult woman, long midnight hair with blue silver tips, red eyes, small broken silver crown, fitted black armored Victorian dress with wine-red lining, thigh-high armored boots, huge raven feather wings, long black silver scythe with muted crimson crescent blade. No giant background moon. Crisp detailed pixel-art clusters and legible silhouette suitable for game sprites, no smooth photoreal rendering. EXACT4columns x2rows on2048x1024 true transparent canvas, eachcell512square. EVERY pose complete including ALL wings scythe hair crown boots with minimum35px padding from EVERY cell edge, no overlaps. Character anatomical body SAME SCALE every frame: head at localy150 and boots at y450 for upright poses, body centered localx256. Wings may rise to y45, scythe tip fits x35..477,y35..465. Row1: 0hover wings halfway raised scythe held diagonally;1hover wings lower;2hover wings fully raised;3hover wings down-back. Row2:4windup for diving scythe slash, bent knees wings swept back blade overhead;5release diving diagonal scythe sweep, leaning but same body size;6one hand extended shooting feather blades scythe held near side wings half spread;7both wings broadly spread invoking night veil scythe held vertical beside body. Three-quarter view facing RIGHT consistently allcells. Feet baseline y450 onbothrows despite bent action poses, consistent pelvis root localx256. Isolated full figures no cast groundshadow no effects trails no letters grid or labels. Never enlarge action bodies or crop wings.

### bodyPadding

Repair this sprite atlas for safe game slicing. Preserve EXACT same character identity, all8 poses, colors and 4columns2rows arrangement, samecanvas aspect and resolution. Uniformly shrink EACH entire isolated character including wings hair crown and scythe to 82% of current size, all poses same reduction, recenter EACH inside its own equal grid cell. Ensure complete wings and weapons in every pose with minimum25px empty transparent gutter to every cell boundary. Restore any clipped outer feather or blade tip at cell edges. Feet in each row align consistent horizontal baseline insidecell around90%height. Keep anatomical body dimensions consistent acrossallposes. No other changes, no extra figures or effects no text labels borders. Preserve TRUE TRANSPARENT background. Pixel sprite artwork stays sharp and colored edge matte removed.

### portraits

Transparent production dialogue portrait atlas1536x1024 EXACT3columns2rows. Reference is final sprite design of Noctis: mature adult raven-winged queen, long black hair with silver-blue tips, small broken silver crown, crimson eyes, black Victorian armor dress, ruby throat jewel, black feather wings, restrained wine-red accents. Create six consistent waist-up portraits with face toward viewer: row1 neutral solemn, cold mocking smirk, furious commanding; row2 shocked wide eyes, pained wounded pride handtochest, resigned sad calm lookingdown. Same face same adult proportions consistent headscale and centered composition EVERYcell, minimum20pxmargin top andsides, crown andhair fullyinsidecell, bustendsnearbottom. Wings only smallshoulderportion to avoid covering face. Elegant highquality gothic anime illustration, detailed eyes andfabric, no seductive pose or exposed torso, no giant weapon acrossface. Transparent background no scene no text labels grids or extra people. Same identity as sprite but polished face portrait.

### portraitPadding

Repair ONLY atlas padding. Preserve same six Noctis portraits same identity expressions 3columns2rows1536x1024 and transparent background. Reduce each full bust uniformly to90% aroundcellcenter, same reduction allsix, put entire crown within eachcell minimum24pxtransparenttopmargin, minimum18pxsidegutter. Reconstruct missing crown tips naturally. Retain shoulder/bust bases, leave transparent margin between cells. No change to face design eyes expression costume hair or colors. No background panels words labels. Truealpha transparency.

### effects

Production game VFX asset atlas 1024x1024 TRUE TRANSPARENT, EXACT2x2equalcells, each object isolated and fully inside its cell with30pxgutter, centered. Gothic detailed PIXEL ART small crisp clusters, cold blue/silver with small wine-red accents. Cell0 top left: a SINGLE long raven feather pointing horizontally RIGHT (quillleft, tipright), black violet vanes with sharp silverblue edges, slight arc; use large clear silhouette not cloud. Cell1 top right: a thin crescent SCYTHE SWEEP arc open to LEFT, whiteblue cutting edge, wine-red innerglow, tapered streak tips and 3 tiny loose feathers; nearcircular arc fits fullcell. Cell2 bottom left: ECLIPSE disk, dark opaque nearblackcircle surrounded by thin luminous silverblue corona with subtle pinkrim, dramatic feather-shaped edge wisps, transparent outside disk. Cell3 bottom right: a loose spiral wreath of black raven feathers and cold blue wisps, transparent EMPTY middle and outside, no opaque background, useful rotating nightveil particles. No people no text no gridlines no background no additionalcopies. Fine pixel art for matching a black winged scythe queen boss.

