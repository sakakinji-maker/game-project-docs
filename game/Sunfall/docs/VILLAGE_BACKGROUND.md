# 불씨 마을 고딕 배경 (2026-10-27)

사용자가 제공한 안개 낀 중세 도시 이미지를 건축·분위기 참고로 삼아, 내장 image_gen 도구로 횡스크롤용 도트 배경을 새로 생성했습니다. CLI/API fallback은 사용하지 않았습니다.

## 파일과 적용

- 참고 이미지: `C:/Users/sakak/AppData/Local/Temp/codex-clipboard-b840ebf2-c8b2-4fc4-814c-098958b8afb3.png`
- 게임 에셋: `Assets/Sunfall/Resources/World/EmberVillageV1.png`
- 로드 경로: `World/EmberVillageV1`
- 시작 마을에서 1280×720 전체 배경으로 표시합니다. 왼쪽 대장간, 중앙 등불 쉼터, 오른쪽 상점·성문을 기존 NPC 위치에 맞춰 구성했습니다.
- 수평 돌길은 화면 높이 75% 부근이며 NPC와 주인공의 기존 발 기준 Y=540을 유지했습니다.
- 태양 없는 세계의 청회색 안개·고딕 지붕·성당 실루엣과 따뜻한 창문·화로 불빛을 조합했습니다.
- 기존 네모 건물·작업대·성문은 새 배경이 있으면 그리지 않습니다. 리소스 임포트 중에는 기존 배경을 표시하고 재시도합니다.
- 상하단 UI에 어두운 반투명 바탕을 적용했습니다. NPC 이름은 후속 수정으로 아래 이름표 방식으로 변경했습니다.
- 기존 WorldArtImporter의 Point / Clamp / NPOT 유지 / 밉맵 없음 / 무압축 / 최대 4096 설정을 사용합니다. 메타 파일을 함께 저장했습니다.

## 확인 범위

2026-10-28 이름표 UI: NPC 위의 큰 네모 대화 버튼과 이름 바탕을 제거하고, 어두운 외곽선이 있는 작은 이름·얇은 금빛 장식으로 교체했습니다. 가까운 대상에만 `Space · 대화`(터치에서는 `터치 · 대화`)를 표시하며 이름표 클릭도 지원합니다. 캐릭터 자세에 따라 높이가 흔들리지 않습니다. Core / Runtime / Editor / Tests 컴파일 성공, 오류·경고 출력 없음. 로그: `Temp/NpcNameplateCompile/compile.log`. 플레이·QA는 하지 않았습니다.

Core / Runtime / Editor / Tests 컴파일 성공, 오류·경고 출력 없음. 로그: `Temp/VillageBackgroundCompile/compile.log`. 실제 플레이, QA 및 자동 테스트는 실행하지 않았습니다. 원본 PNG 크기는 1672×941입니다.

## 생성 프롬프트

```text
Use case: stylized-concept. Asset: finished opaque widescreen 16:9 pixel-art village environment for SUNFALL, a 2D side-scrolling dark fantasy action game. Input image 1 is an architectural and atmosphere reference: reinterpret its dense Gothic rooftops, distant cathedral spires, damp stone, fog and warm windows as a NEW original game background. Change the elevated deep alley viewpoint to an eye-level side-on horizontal town square, suitable for characters walking left to right. Precise layout for existing gameplay: blacksmith workshop around 17% image width with a small amber forge and chimney; a modest lantern shelter and extinguished circular sun emblem around 38%; merchant storefront with dark burgundy cloth canopy around 61%; an open stone gateway at 88% leading toward a blue-green forest. Keep each location visibly distinct but cohesive with medieval half-timber and dark stone architecture. Background layers: desaturated distant cathedral and clustered spires in pale teal-grey mist, middle-distance tiled gables, front-facing houses. The sun has vanished: no visible sun, no bright sunset or daylight. Warm amber window lights and small lanterns are the only warm accents. Style: detailed handcrafted PIXEL ART, crisp pixel clusters, controlled palette of blue charcoal, slate grey, muted teal and weathered brown with amber accents; no painterly blur or photorealism. Composition: roof silhouettes fill upper half; building bases around 69% image height; ONE flat unobstructed horizontal cobblestone walking lane at exactly 75% image height extending across full width. Lower 25% is darker paved foreground, with small puddles reflecting windows but no obstacles. NPC silhouettes will be drawn in front by the game: reserve quiet readable space at 17%, 38%, 61% across, between 62% and 75% of image height. No stairs or railings across the walking lane, no foreground buildings obstructing it. Upper 18% is relatively quiet foggy sky for UI. No people, characters, monsters, weapons, text, signs with letters, UI, borders, watermarks or logos. Fully opaque landscape background, ideally 1920x1080 or similar 16:9.
```
