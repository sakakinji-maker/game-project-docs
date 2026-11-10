# 주인공 아트 — 2026-10-26

> 최신 공격 애니메이션은 `COMBAT_ART_V2.md`를 참고하세요. 아래의 통합 시트 공격 프레임·영역 분할 설명은 최초 버전 기록입니다. 현재 검 3연타와 공중 베기는 별도의 V2 PNG를 사용합니다.

## 적용한 디자인

- 얼굴·머리: 제공된 `캐릭 디자인.jpg`의 붉은 갈색 머리, 앞머리, 긴 옆머리와 낮게 묶은 번 헤어.
- 의상: `옷디자인.jpg`의 흰 드레이프 블라우스, 검은 긴 장갑, 넓은 벨트와 검은 하의. 전투용 검은 부츠로 마감.
- 검 동작: `검쓰는거.jpg`의 준비·베기·회수 자세와 검 궤적을 참고하여 세 가지 연속 공격으로 제작.
- 제공된 이미지들은 디자인 참고로 사용했으며 원본 파일은 수정하지 않음.

## 저장된 에셋

- `Assets/Sunfall/Resources/Hero/HeroPortrait.png`: 1024×1536 RGBA 대화용 초상화.
- `Assets/Sunfall/Resources/Hero/HeroAtlas.png`: 1024×1536 RGBA, 24개 도트 자세.
- 생성 방식: 내장 image_gen 도구. CLI/API 별도 호출 없음.

## 게임 연결

마을 및 전투의 기존 도형 주인공을 도트로 교체했다. 좌우 방향은 이미지 반전으로 처리하고 발 위치를 기준으로 프레임을 배치한다. 실제 생성 시트의 비균등 여백을 고려한 픽셀 영역을 `SunfallGame.Hero.cs`에 정의했으며, 인접 검 궤적이 가까운 두 프레임은 영역을 나누어 표시한다.

| 프레임 | 동작 |
|---|---|
| 0–3 | 대기 |
| 4–7 | 달리기 |
| 8–11 | 검 1타 |
| 12–15 | 검 2타 |
| 16–19 | 검 3타 |
| 20 | 공중 베기 |
| 21 | 구르기 — 한 자세를 회전하여 표시 |
| 22 | 마법 시전 |
| 23 | 피격 |

지상 검 동작의 준비·접촉·회수 프레임은 RunModel의 타격 상태에 동기화한다. 공중 이동에는 달리기의 접힌 다리 자세를 사용한다. 정지 및 대화 중에는 이동 애니메이션 시간이 멈춘다. 기존 피해량, 이동 거리, 무적과 쿨다운 수치는 유지한다.

대화창 왼쪽에는 여행자의 초상화, 오른쪽에는 발화자의 이름과 대사를 표시한다. 아린의 기존 3페이지 뒤에 여행자의 답변 1페이지를 추가했다. NPC 아린의 외형은 여행자와 구분하여 유지한다.

`HeroArtImporter.cs`는 알파 채널, 무압축, 원본 크기, 밉맵 끄기와 도트 Point 필터를 적용한다. 초상화에는 Bilinear 필터를 사용한다.

## 확인 범위

PNG 크기·알파 채널과 생성 이미지 구성을 확인했다. Unity 6000.3.27f1 내장 컴파일러와 Bee 참조 설정으로 Core / Runtime / Editor / Tests 네 어셈블리를 컴파일했다. 출력 위치는 `Temp/CharacterCompile`이다.

본인 요청대로 Unity Play 모드, 자동 게임 실행, 플레이 및 게임 QA는 수행하지 않았다. 기존 대화 3페이지 관련 기록은 이 변경 이전 기록이다.

## 사용한 프롬프트

### 표시 누락 수정 기록

2026-10-26 본인 확인에서 캐릭터가 보이지 않는 문제가 보고되었다. Unity 에셋 진단 결과 두 PNG의 실제 타입이 `UnityEngine.Cubemap`이어서 `Resources.Load<Texture2D>`가 null을 반환했다. 기존 임포터는 `textureType`만 지정하고 `textureShape`를 지정하지 않았다.

`TextureImporterShape.Texture2D`를 명시하고 기존 에셋도 재임포트했다. Unity 편집기에서 도트 시트는 `1024x1536 RGBA32 Point`, 초상화는 `1024x1536 RGBA32 Bilinear`의 Texture2D로 로딩되는 것을 확인했다. `Sunfall → Check Hero Resources (No Play)` 메뉴로 다시 조회할 수 있으며 결과는 `Temp/HeroResourceDiagnostics.txt`에 기록된다.

스크립트 재로딩 후 OnEnable에서 그림과 터치 입력을 복구하고, 임포트 중 그림이 없으면 재시도·임시 표시를 하도록 수정했다. 좌우 반전과 회전은 논리 화면 좌표계에서 합성하며 UV는 원본 PNG 크기를 기준으로 정규화한다. 네 어셈블리 컴파일 성공. Play 모드를 시작하거나 게임 조작을 하지는 않았다.

### 초상화
Use case: stylized-concept. Asset type: transparent anime dialogue portrait for a 2D dark fantasy action game. Reference image 1 defines the adult heroine's identity: muted burgundy red hair, straight bangs, long face-framing locks, low messy tied bun, rose red eyes, pale skin, reserved confident expression. Reference image 2 defines CLOTHING ONLY: draped ivory white off-shoulder blouse, glossy black opera-length gloves, black high-waisted tailored trousers, wide black belt with small brass buckle. Combine identity of reference 1 with clothing of reference 2. Do not copy blonde hair. Draw a polished original game portrait, clean delicate ink lines and soft cel shading, subtle cool shadows, readable face. Single adult woman, head to upper thighs, three-quarter view facing slightly toward viewer's right, head fully visible, calm alert gaze, one gloved hand resting near waist and other naturally relaxed. Keep hair silhouette and clothing exceptionally faithful. Include belt and top of black trousers. No cigarette, no smoke, no lettering, no watermark, no scenery, no frame. Actual transparent background with clean alpha. Portrait canvas 1024x1536; generous margin around hair and elbows; lower body naturally cropped at upper thighs.

### 도트 시트 초안
Use case: stylized-concept. Asset type: production pixel-art animation sprite atlas for a 2D side-scrolling sword action game. References: image 1 is sword motion/pose reference ONLY, especially overhead windup, forward sweeping cut, reverse rising cut and broad overhead finisher; image 2 is the adult heroine identity (burgundy red low messy bun, straight bangs, long sidelocks, pale skin); image 3 is CLOTHING ONLY (ivory draped off-shoulder blouse, black opera gloves, black high-waisted trousers, broad black belt brass buckle). Create ONE consistent adult swordswoman with those features, black boots and one silver single-handed sword. Right-facing side view for every frame. Crisp genuine low-resolution pixel art, deliberate pixel clusters, dark outline, limited 20-color palette, no smooth painting, no antialiasing, no blur. Actual transparent background. No labels, numbers, grid lines, scenery, shadows on ground, watermarks, or stray marks.
CRITICAL ATLAS: exactly FOUR columns and SIX rows = 24 separate poses in an invisible uniform grid on a 1024 by 1536 canvas. Each cell is exactly 256 by 256. Each sprite's body has identical scale, standing height roughly 144 pixels, hip horizontal position at local x=112, feet baseline at local y=224. Keep full sword and any arc within that cell, minimum 12 pixel empty margins, do not overlap cells. Character must be full-body in every cell. No visible grid.
Row 1 (top): four idle / breathing frames, sword held low forward; subtle bun and blouse movement.
Row 2: four distinct running cycle frames facing right, readable alternating legs, sword trailing, consistent body scale and baseline.
Row 3: first sword hit four sequential frames: rear/upward windup, fast forward diagonal downward cut (contact), follow-through forward (contact), return. Based on first reference overhead-to-forward sequence. White pale gold arc ONLY on middle two frames, confined within cell.
Row 4: second sword hit four sequential frames: low rear windup, reverse rising horizontal cut (contact), extended reverse sweep (contact), recovery. Different silhouette from row 3. White pale gold arc ONLY on middle two frames.
Row 5: third sword hit four sequential frames: raised overhead sword windup, powerful broad downward crescent slash (contact), deep forward follow-through (contact), recovery. Larger white pale gold arc but still wholly contained in its cell. No extra limbs.
Row 6: four DIFFERENT utility poses in order: airborne forward horizontal sword slash with white arc, tucked forward roll with sword held safely close, standing magic cast with free black-gloved hand extended and tiny orange ember, recoil hurt pose. These four are separate states, not a cycle.
Emphasize consistent red bun / white blouse / black long gloves and trousers in ALL 24 frames. Match movement reference's clear silhouette while using the new heroine. Preserve exact grid and transparent gutters.

### 도트 시트 최종 수정
Edit this sprite atlas for strict game-engine usability. KEEP the same red-haired adult woman, white off-shoulder blouse, black long gloves, black trousers, boots, silver sword, 24 poses, and order (4 idle / 4 run / 4 first sword combo / 4 second sword combo / 4 third sword combo / airborne slash, roll, cast, hurt). KEEP canvas 1024x1536, EXACTLY 4 columns by 6 rows, 256x256 cells. IMPORTANT CHANGE: REMOVE ALL smoky gray backgrounds, auras, hazes, gradient glows, drop shadows and bloom. Every background pixel must have alpha ZERO. Sprites are hard-edged pixel art with opaque colored pixel clusters, no semitransparent halo. Sword arcs are SOLID white pixel crescents with pale gold thin edge, not glowing clouds. Tiny orange magic ember allowed; no blood droplets. Make every pose SMALLER and better spaced: each complete character+sword+arc fits inside the centered 192x192 area of each 256x256 cell, leaving at least 32 pixels of fully transparent margin on all four sides. No limb, sword or arc crosses a cell boundary. Head-to-feet height of standing character is 128 pixels, consistent in every pose. All standing foot baselines at local y=208, hips near local x=112. Across the atlas the foot baselines are therefore y=208,464,720,976,1232,1488. Align idle characters exactly; keep running and attack feet on these baselines. 3 sword attack rows must retain distinct motion. No extra elements, no text, no visible cell lines. The central task is removing haze and enforcing invisible uniform 4x6 grid spacing with smaller pixel characters.

