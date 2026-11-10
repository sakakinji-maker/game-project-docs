# 공격 도트 수정 V2 — 2026-10-26

## 문제와 변경

이전 24자세 통합 시트는 인접한 검 궤적이 겹치고, 일부 프레임을 두 영역으로 잘라 그려서 검 끝이 잘릴 수 있었다. 프레임마다 다른 발·몸통 기준점도 화면상 위치 변화의 원인이었다. 점프 공격은 접촉 순간의 도트 한 장만 사용하고 준비·회수에는 달리기 자세를 사용했다.

공격을 네 개의 별도 PNG로 분리했다. 임의 격자나 영역 분할 대신 실제 불투명 실루엣의 경계에 여백을 더한 온전한 사각형을 사용한다. 지상은 발바닥 높이와 골반 중심을 맞추고, 공중은 움직이는 발 대신 골반 높이를 맞춘다. 실제 보간된 중간 그림을 추가하는 방식은 아니며, 각 공격은 네 자세로 표시한다.

- 검 1타: 준비 / 내려베기 / 앞으로 베기 / 낮은 회수.
- 검 2타: 낮은 준비 / 올려베기 / 위쪽 회수 / 머리 위 준비.
- 검 3타: 머리 위 준비 / 큰 내려베기 / 낮은 후속 궤적 / 경계 자세.
- 공중 공격: 공중 준비 / 가로 베기 / 후속 궤적 / 공중 회수.

연타 경계에서 2타 준비는 1타 마지막 자세, 3타 준비는 2타 마지막 자세를 재사용하여 경계의 위치·크기 불연속을 줄인다. 피격 중에도 진행 중인 검 자세는 유지하고 색으로 피격을 표시한다. 공중 공격이 끝나면 착지 또는 다음 행동까지 공중 회수 자세를 유지한다. 검의 피해량, 사거리, 타격 순간과 공격 간격은 바꾸지 않았다.

## 저장된 에셋

모두 `Assets/Sunfall/Resources/Hero/Combat/` 아래에 저장했다.

| 파일 | 원본 크기 | 자세 수 |
|---|---|---:|
| Combo1V2.png | 2172×724 | 4 |
| Combo2V2.png | 2172×724 | 4 |
| Combo3V2.png | 1254×1254 | 4 |
| AirSlashV2.png | 1254×1254 | 4 |

이전 HeroAtlas.png는 대기·달리기·구르기·시전·피격에 계속 사용한다. 대화 초상화는 변경하지 않았다. 잘라 쓴 예전 공격 자세는 런타임 목록에서 제거했다. 새 PNG를 다운스케일하지 않도록 공격 에셋은 최대 크기 4096, Point 필터, 무압축, Texture2D 및 원본 알파 설정을 적용한다.

`SunfallGame.SwordArt.cs`에 전체 프레임 영역, 기준점, 원본 크기와 표시 배율이 있다. PNG 파일을 바꾸면 이 좌표도 함께 갱신해야 한다.

## 확인 범위

이미지의 투명 영역과 실루엣 범위를 읽어 프레임 좌표를 정리했다. Unity 내장 컴파일러와 프로젝트 참조로 Core / Runtime / Editor / Tests 네 어셈블리 컴파일 성공. 결과는 `Temp/CombatArtV2Compile`에 있다. 본인의 요청대로 Play 모드, 자동 전투, 게임 플레이 및 게임 QA는 실행하지 않았다. 실제 타격감과 프레임 연결의 최종 평가는 본인 QA가 필요하다.

## 생성 방법과 프롬프트

내장 image_gen 도구를 사용했다. 기존 HeroAtlas와 생성된 직전 공격 시트를 디자인 참고로 전달했다. 첫 3타 가로 시트는 궤적 간격이 좁아 채택하지 않고 2×2 시트로 다시 생성했다. 프로젝트에는 최종 선택한 네 PNG를 저장했다.

### 검 1타
Create a replacement PIXEL ART animation strip for the heroine in the reference, not an illustration. Same adult woman, burgundy bun and bangs, white off-shoulder blouse, black long gloves, black high-waist pants, black boots, one silver sword. Right-facing side view. Exactly FOUR sequential full-body frames in ONE horizontal row, evenly separated. Canvas 1536x512 (3:1). Four invisible cells 384x512; every character, sword and SOLID pale ivory slash arc stays within its own cell leaving minimum 40 pixels of empty margin on every side. All four figures same scale (head-to-heel approximately 200 pixels), same hip-root horizontal position local x=170, both grounded heels on the same baseline local y=400. Actual fully transparent background, hard crisp pixel edges, dark outline, no gradient glow or haze, no text, labels, boxes, ground, grid or shadows. This is GROUND COMBO HIT 1, a flowing downward-forward sword cut. Frame 1: upright combat guard with sword lifted behind head, knees slightly bent. Frame 2: blade actively swings diagonally down-forward through chest height with modest crescent trail. Frame 3: blade completes forward-low sweep, body leaning forward slightly, trail moves with blade. Frame 4: low-forward recovery with sword ready to rise for second hit; no arc. Keep pelvis and feet steady, no horizontal teleport, no changes of body size, modest continuous pose changes, NEVER crop a sword or foot. The reference is identity/style ONLY; do not reproduce its crowded atlas layout. Each frame isolated in generous transparent space.

### 검 2타
Create the NEXT FOUR FRAMES after this reference animation strip: COMBO HIT TWO, a smooth low-to-high reverse rising sword cut. Preserve this EXACT pixel-art adult heroine, outfit, scale, proportions, palette and crisp rendering. Burgundy bun, ivory off-shoulder blouse, black opera gloves, black pants and boots, silver sword. Right-facing. Four sequential full-body poses in ONE horizontal row on a 3:1 transparent canvas. IMPORTANT: leave a wide transparent gap between each complete silhouette INCLUDING ALL sword arcs, do not touch neighbouring frames or canvas borders. Frame 1 closely matches last reference pose: sword held low in front/right, knees bent. Frame 2: swing rises from low right to high right through waist and chest; clear single crescent. Frame 3: follow-through finishes with sword raised above the right shoulder, upper arc. Frame 4: sword raised overhead, settling into the windup for a third downward finisher. The character does NOT turn left. Same foot baseline, same head/torso size, stable planted feet and hip root. Small continuous changes between poses, not four unrelated attacks. Grounded feet, no jumps. Whole blade, all fingers and all boots visible, generous transparent outer margin. Pixel-art limited palette, hard opaque pixel clusters, no bloom, haze, background, floor, text, numbers, grid or frame.

### 검 3타 초안
Create the NEXT FOUR sequential frames following the attached pixel-art animation strip: GROUND COMBO HIT THREE, a heavy overhead downward finisher. Preserve EXACT same adult swordswoman, burgundy bun/bangs, ivory off-shoulder blouse, black long gloves, black pants/boots, silver sword, outline, pixel scale and body proportions. Right-facing. 3:1 transparent canvas, ONE ROW of FOUR completely separated full-body frames. Each whole sword+body+arc must have 35+ pixels of clear margin to neighbouring frames and canvas edges; do NOT crop anything. Frame 1 MATCHES last reference pose: sword raised over head, grounded combat stance. Frame 2: powerful diagonal downward cut to the forward right, broad crisp ivory/gold crescent, body leaning into cut. Frame 3: low forward follow-through, knees flexed, sword finishes forward-low with remaining short arc. Frame 4: recover to balanced right-facing ready stance, torso upright, sword held diagonally forward-down, no arc; this must transition to the original idle pose. Stable hip horizontal root, identical standing body scale and heel baseline across all frames; changes in crouch are natural joint movement only. This is a continuous sequence, no left-facing pose, no teleporting. Crisp game pixel art, opaque pixel clusters, no glow, no background haze, no shadows, no labels/numbers/grid.

### 검 3타 최종
Re-layout and refine this four-frame ground sword finisher as a SQUARE TWO BY TWO animation sheet. Same exact four poses in reading order, same pixel heroine identity, body scale, right-facing stance and sword. The original horizontal strip crowds the second crescent against the third boot; FIX this by placing each full pose in its OWN quadrant with a very large completely transparent gutter between quadrants, all art confined inside each quadrant. Keep entire sword and ivory crescent intact, no crop. Frame 1 top-left: overhead windup. Frame 2 top-right: large downward crescent contact. Frame 3 bottom-left: low forward follow-through, small low arc. Frame 4 bottom-right: upright return to idle guard, sword low forward. Feet planted on matching local baselines. Same relative pelvis location across quadrants; identical height/scale. Adult burgundy-haired bun swordswoman, white draped blouse, long black gloves, black trousers and boots. All sprites isolated with transparent background, no text, no borders, no background gradients, no labels, no ground shadows. Do not introduce new actions. Pixel art only.

### 공중 베기
Create a FOUR FRAME airborne sword slash animation sheet for this exact pixel-art adult heroine: burgundy bun and bangs, white off-shoulder blouse, black elbow-length gloves, black trousers and boots, silver sword. All facing RIGHT. A square transparent canvas, exactly 2 columns x 2 rows, FOUR ISOLATED full-body poses. Large EMPTY gutters: every complete character AND sword trail fits within central 70 percent of its quadrant, never touches any neighbour or canvas edge. Same body scale throughout. She is airborne with tucked legs, no floor. Keep the BELT/PELVIS at exactly the same local point in all four quadrants; do not move the whole body to simulate jumping. Frame reading order top-left to bottom-right: (1) windup, tucked legs and sword back above shoulder, no arc; (2) active horizontal forward sword sweep across chest height, long clean ivory crescent reaching right; (3) follow-through, extended blade across the front and fading solid shorter arc, visibly different from 2; (4) recovery, blade returned to low forward guard, legs still tucked ready to land, no arc. Show a complete committed swing; no return to running pose. Crisp pixel clusters, same style as reference, no antialias blur, no glow or haze, fully transparent background, no text, no grid, no labels, no extra limbs.

