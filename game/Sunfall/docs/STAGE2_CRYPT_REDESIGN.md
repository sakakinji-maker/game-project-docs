# 2스테이지 회랑·모르가 개편 · 2026-11-02

2026-11-03: 배경·발판·봉인 구조물은 1스테이지 도트 스타일로 다시 교체했습니다.
현재 적용 내역은 `STAGE2_PIXEL_STYLE.md`를 참고하세요. 아래는 이전 작업 기록입니다.

## 요청과 미술 방향

첫 번째 사용자 이미지의 축축한 청록색 석조 회랑과 두 번째 이미지의 검은 껍질·창백한 내부 몸체·뿌리 같은 섬유를 기준으로 제작했습니다.
사용자가 최종적으로 모르가 외형도 같은 유형으로 변경하도록 요청해, 기존 후드/갑옷 악마를 껍질형 비인간 악마로 교체했습니다.
이름, 왕실 사슬의 간수 설정, 공격 패턴·피해·체력·2페이즈 규칙은 유지합니다.

## 적용 이미지

모두 Sunfall/Assets/Sunfall/Resources 기준입니다.

| 파일 | 내용 | 실제 크기 |
|---|---|---|
| World/Stage2CryptRoomsV1.png | 전투방 3개 + 보스방, 위에서 아래로 4개 파노라마 | 1086×1448 RGB |
| World/Stage2CryptLedgeV1.png | 회랑과 같은 재질의 석조 발판 | 2172×724 RGBA |
| World/Stage2BrokenSealV1.png | 닫힌 봉인 / 열린 잔해, 좌우 2칸 | 1536×1024 RGBA |
| Demons/Stage2MorgaHuskV2.png | 모르가 대기·이동·사슬 공격·구속·폭풍 8포즈 | 1774×887 RGBA |
| Story/BossPortraits/MorgaHuskExpressionsV2.png | 새 외형의 대화 표정 6종 | 1536×1024 RGBA |

전투방은 성문 회랑 → 사슬 감옥 회랑 → 왕실 봉인 회랑 순서입니다. 보스방은 빈 중앙 벽과 높은 천장이 있는 묘실입니다.
이전 성소 배경의 색상 변경과 코드로 덧그린 망루/기둥/문양은 2층에서 사용하지 않습니다.
다른 층의 배경은 변경하지 않았습니다.
기존 전투방 3개와 보스방의 발판 좌표·몬스터 배치·진행 조건은 유지하고 석재 표현만 교체했습니다.

## 등장 연출

- 0~1.25초: 봉인 껍질을 감싼 4가닥 사슬이 팽팽해짐.
- 1.25초: 사슬 절단, 연결부 파편이 흩어지고 고정된 끝은 반동으로 휘어짐.
- 1.25~1.8초: 닫힌 껍질에서 열린 잔해로 전환.
- 1.25~2초: 모르가가 드러나면서 지면으로 내려옴.
- 2.05초 이후: 전투 대기 자세.
- 2.35초: 첫 등장 대사. 기존 1.8초보다 늦춰 절단 장면이 대화창에 가려지지 않게 함.
- 기존 총 등장 시간 3.6초와 대화 중 시간 정지 규칙 유지.
- 전투 중 열린 잔해는 낮은 밝기로 남으며, 처치 후 금빛 봉합 자국을 표시.

전체 화면 흔들림은 추가하지 않았습니다. 사슬은 기존 실제 사슬 조각 텍스처로 그립니다.
대사 첫 줄과 초상화만 새 외형에 맞췄으며 시청 기록을 초기화하지 않았습니다.

## 아틀라스 보정

최초 보스 도트는 공격 팔/사슬 여백이 부족하여 이미지 생성 도구로 여백을 추가한 버전을 최종 채택했습니다.
최종 PNG의 각 칸 알파 경계를 확인했고, 위 행 발 기준점 421px / 아래 행 828px를 사용합니다.
모든 포즈에 동일 배율 0.40을 적용합니다. 프레임별로 몸 크기를 늘려 맞추지 않습니다.
석재는 실제 불투명 영역을 기준으로 원본의 280~497행을 사용해 보이는 발판 윗면과 충돌 높이를 맞췄습니다.
봉인 밑받침은 원본 951행을 지면에 정렬합니다.

## 확인

이미지 크기·알파·8칸 여백·발 기준점과 Resources 경로를 확인했습니다.
tools/Compile-AttackArt.ps1 -OutputFolder Temp/Stage2CryptCompile
Core·Runtime·Editor·Tests 모두 exit=0.
로그: Temp/Stage2CryptCompile/compile.log.
Unity Play·QA·테스트 실행은 하지 않았습니다.

## 생성 프롬프트

도구: 내장 image_gen.imagegen. 사용자 참고 이미지를 referenced_image_paths로 전달했습니다.
배경만 transparent_background=false, 나머지는 true입니다. 생성 원본은 .codex/generated_images에 보존했습니다.

### CryptRooms
참조: C:/Users/sakak/AppData/Local/Temp/codex-clipboard-9cd0afde-b83c-4a2d-9dee-c119fe2474a5.png

```text
Use case: illustration-story. Reference image is the EXACT architectural atmosphere: damp cracked blue-green stone pointed gothic vaults, columns, moss and desaturated pale green light, hand-painted dark fantasy. Create a game BACKGROUND ATLAS 1536x2048 portrait. Exactly FOUR horizontal panorama strips stacked vertically, each 1536x512, without gaps or borders, NO text. Each strip is a coherent separate room, not a continuous vertical image. 2D side-scrolling platform game view, camera looking sideways at the rear wall, not a deep central vanishing-point corridor. Walls fill each strip; bottom 15% has dark stone base but NO playable foreground platforms (added separately). Keep moderately dark and low contrast behind action; beautifully detailed stones with crisp painterly pixel-like edges, no bright neon. Top strip: ruined outer gate hall with massive arches and pale green light through barred openings. Second strip: long prison corridor, repeated rib vaults and recessed iron bars, long heavy dark chains hanging in the recesses. Third strip: fractured inner fortress nave, thick cracked piers, sealed royal doors, cold fog near ground. Fourth strip: enormous boss crypt, high ancient vaults, blue-black niches, pale teal backlight, a broad EMPTY central wall and floor area for a separately placed seal cocoon and boss; no drawn creature or cocoon. The four strips share one cohesive architectural material palette, no characters, no logos, no UI, no words, no painted gameplay floor ledges. Each strip edge straight and exact at y512 y1024 y1536.
```

### MorgaHusk
참조: C:/Users/sakak/AppData/Local/Temp/codex-clipboard-cee0ef7e-b68a-44ea-8228-3b4df332054e.png

```text
Use case: identity-preserve. Reference is creature design inspiration. Redesign Morga, the royal chain warden, as a non-human eldritch demon matching the reference: a tall narrow ivory husk humanoid with hollow dark face and a vertical mouth opening, sinewy branch-like tendrils, enclosed between TWO huge split black-and-ivory shell shoulder plates that curve around the body like a cracked cocoon. Dark root-like legs and arms, tarnished bronze wrist shackles and short dangling iron chains. NO human woman's face, NO breasts or sexual anatomy, NO hooded armored human, no blood or gore. Full-body game PIXEL ART animation atlas, EXACT 4 columns x 2 rows equal square cells, 2048x1024 RGBA transparent. Eight sprites of SAME creature facing RIGHT in readable 3/4 side view. Each 512px cell uses identical scale and fixed floor baseline 480px from cell top, feet center x256, top of shell about y65. Keep entire creature, tendrils and short chain weapons within 30px padding. No stretching body during attack. Top row: 1 idle breathing settled, 2 idle slightly tense, 3 walking lead leg, 4 walking opposite leg. Bottom row: 5 chain slam windup raised arms, 6 chain slam follow-through leaning forward with chain aimed down right, 7 binding spell arm extended right, 8 chained storm cast arms open with tendrils spread. Crisp dense pixel art suited to dark gothic 2D action game, ivory form readable against teal dungeon, restrained blue-black and tarnished gold palette. True transparent background, no scenery, no ground shadow strip, no text, no grid lines, no effects outside cells.
```

### Seal
참조: C:/Users/sakak/AppData/Local/Temp/codex-clipboard-cee0ef7e-b68a-44ea-8228-3b4df332054e.png / C:/Users/sakak/AppData/Local/Temp/codex-clipboard-9cd0afde-b83c-4a2d-9dee-c119fe2474a5.png

```text
Use case: illustration-story. Create TWO isolated scenery props in an exact 2 columns x 1 row atlas, 1536x1024 RGBA. Each 768x1024 cell centered at x384 with baseline y960 and matching scale, 45px transparent margin. Reference1: giant cracked black outer shell with pale stone inner surfaces and thin black root filaments. Reference2: damp blue-green gothic masonry style. LEFT cell: CLOSED ancient upright cocoon shrine, two thick pale ivory shell plates sealed together with a dark vertical hairline fissure, rooted into a small ruined stone plinth, a bronze royal seal at center. RIGHT cell: SAME cocoon shrine BROKEN OPEN, shell plates spread left and right exposing a dark empty hollow and torn tendrils, broken bronze royal seal at bottom. Empty props only, no creature inside, no characters, no prepainted chains (chains animated separately). Dark gothic dense painterly pixel art matching a 2D side-view game, sharp readable outer silhouette, black and ivory shell with cold teal edge light, no bright glow, no text, no grid, true transparent background.
```

### StoneLedge
참조: C:/Users/sakak/AppData/Local/Temp/codex-clipboard-9cd0afde-b83c-4a2d-9dee-c119fe2474a5.png

```text
Use case: illustration-story. Create one isolated horizontal stone platform tile for a side scrolling gothic crypt game. Reference for blue-green aged cracked stone, moss in seams, hand-painted texture. Canvas 1536x512 RGBA transparent. Single long straight rectangular stone ledge occupies x0 through1536 and y100 through420. Perfectly FLAT level top surface at y100, no objects or grass sticking above it. Side view chunky cracked dressed stone blocks, dark desaturated teal shadows, restrained moss along lower joints, slightly worn pale green upper lip, darker underside. Seamless repeatable left/right ends, no tapered ends, no perspective slope, no scenery, no supports, no text, no characters. Dense crisp pixel-art edges fitting gothic 2D action sprites. True transparent above and below ledge.
```

### 최종 도트 여백 보정

참조: 최초 생성 MorgaHusk 시트.

```text
Use case: identity-preserve. Edit this exact creature sprite atlas to FIX ALL CELL PADDING AND FOOT ALIGNMENT. Preserve creature design, eight pose meanings, dark pixel-art rendering and palette. Final exactly 4 columns x2 rows equal square cells on 2048x1024 transparent canvas. IMPORTANT make each sprite SMALLER within its own cell: each body and all tendrils, hands and chains must fit ENTIRELY within local x64..448 and y64..456. Keep at least 64px completely transparent margin at left and right of EVERY cell, 56px at bottom. Absolutely NO overlap into neighboring cells, no clipping of hands/chains at edges. Feet baseline local y456, foot pivot x256 for EVERY pose. Same anatomical body scale in all eight poses; raised hands must fit by posing, not scaling. The character should occupy about 360px high in idle, never 500px; do not automatically enlarge to fill cell. Eight poses in reading order: idle A, idle B, walk A, walk B; chain slam windup, chain slam followthrough, binding cast arm forward, storm cast arms open. Shorten dangling chains to ensure they fit padded cells. Pose6 lean naturally without extending past local x440. Keep negative space between all sprites. No captions, no gridlines, no background. True transparent.
```

### 대화 초상화

참조: 최초 생성 MorgaHusk 시트.

```text
Use case: identity-preserve. Reference is Morga's new in-game sprite atlas. Create dialogue PORTRAIT ATLAS with SIX FRONT-FACING chest-up portraits of precisely this same NON-HUMAN eldritch creature. The hollow pale ivory inner humanoid with long black vertical mouth, tiny shadowed eyes, dark branching tendrils for its neck/chest, framed by cracked ivory/black cocoon shell shoulders. Bronze royal shackles, cold teal shadows. No attractive human face, no human hair, no breasts, no armored hood, no blood. Keep monster identity absolutely consistent. Dark gothic illustrated painterly shading with clean detailed edges. Exactly 3 columns x2 rows of equal square cells, canvas1536x1024 RGBA transparent. Center head/face at same location and scale each cell, fully contained bust with40px margins. Reading order: calm ominous stare; contempt with slight head tilt and tightened eyes; furious threat with mouth stretched wider and tendrils tense; shocked widened eyes and recoiling neck; pained sagging posture and strained mouth; defeated resigned lowered head and slack tendrils. Convey emotions through head posture, eye slits and mouth without inventing human lips. No full body, no labels/text/borders, no scenery. True transparent background.
```

