# 서장 전체 화면 시네마틱

2026-11-01 후속 수정: 화면 흔들림 피드백에 따라 카메라 이동·확대와 중첩 GUI 행렬을 제거했습니다. 현재 배경은 고정이며 아래의 카메라 연출 설명은 최초 구현 기록입니다. 자막·사슬·입자·별실·전환은 유지됩니다. 개발자 F10 초기화는 `DEVELOPER_RESET.md`를 참고하세요.

2026-11-01. 시작 메뉴의 `이야기 시작`을 누르면 기존 서장 9개 대사를 전체 화면 2.5D 시네마틱으로 재생합니다. 전용 일러스트 4장과 마을 배경에 카메라 이동, 확대, 사슬 흔들림, 불빛, 입자, 별실 곡선 애니메이션을 합성합니다. 인물 프레임 애니메이션이나 음성 더빙이 포함된 영상 파일은 아니며, Unity에서 직접 재생합니다. 기존 배경음은 유지됩니다.

## 장면과 조작

1. 태양의 봉인: 왕실 사슬에 묶인 태양과 대성당, 사슬·재 입자 움직임.
2. 빛을 잃은 세계: 태양 장면이 점점 어두워지고 반대 방향 카메라 이동.
3. 돌아갈 집: 어머니가 주인공을 맞이하는 따뜻한 집, 불빛과 불씨.
4. 나를 딸이라 불러준 사람: 같은 기억에 가까워지는 카메라.
5. 떨어진 손끝: 붉은 균열과 사슬에 끌려가는 어머니.
6. 깨어난 별실: 두 손 사이에 금빛 실이 그려지고 흐름.
7. 살아 있는 심장: 주인공의 결심, 손에서 숲으로 이어지는 별실.
8. 등불을 지키는 이: 불씨 마을을 비추며 아린의 약속.
9. 하늘을 꿰매는 맹세: 주인공과 별실, 게임 제목 등장 후 마을 진입.

장면 길이는 대사 글자 수 / 7.5 + 3초, 최소 7초로 계산합니다. 자막은 0.8초 후부터 초당 22자씩 나타납니다. 완료되면 자동으로 다음 장면으로 넘어갑니다. 모든 장면은 0.75초 암전과 0.9초 페이드 인으로 연결됩니다.

- Space / Enter / 숫자패드 Enter: 자막을 즉시 펼치고, 다음 입력으로 다음 장면.
- 하단 버튼: 같은 동작, 터치 지원.
- Esc 또는 상단 일시정지: 시간·카메라·사슬·입자·자막·전환을 함께 정지. 설정 창도 정지 상태를 유지.
- 서장 건너뛰기: 암전 후 마을로 이동. 이미 전환 중이어도 전체 종료로 변경 가능.
- 정상 종료: 마지막 장면 후 불씨 마을에서 조작 가능. 전환 입력이 마을 대화·이동으로 전달되지 않도록 기존 Block 적용.

진행은 Update에서만 변경하고 OnGUI는 그리기만 합니다. OpeningStory 배열을 재생 중인지로 시네마틱 활성 상태를 판별합니다. 보스 대사와 마을 대화는 기존 대사창을 사용합니다. 타이틀로 돌아간 후 다시 시작하면 서장 시간이 초기화됩니다. 마을과 HUD는 서장 동안 그리지 않습니다.

## 파일과 생성 방식

- 구현: `Assets/Sunfall/Runtime/SunfallGame.OpeningCinematic.cs`
- 연결: `SunfallGame.cs`, `SunfallGame.Story.cs`
- 이미지: `Assets/Sunfall/Resources/Story/Cinematic/` 아래 `BoundSunV1.png`, `MothersHomeV1.png`, `SeveredHandsV1.png`, `GoldenVowV1.png`
- 가져오기: `StoryArtImporter`, 최대 2048px, Bilinear, Clamp, 무압축, Mipmap 끔. 기존 대사창용 Story 이미지는 1024px 설정 유지.
- 원본 매핑: `OPENING_CINEMATIC_SOURCES.json`. 값에 `exec-`와 `.png`를 붙인 파일이 `C:/Users/sakak/.codex/generated_images/01a17569-bfbf-7233-a71a-72eece5bde92/`에 있습니다.

내장 image_gen으로 각각 생성하고 생성 원본을 수정 없이 프로젝트에 복사했습니다. CLI/API fallback 없음. 아래는 최종 프롬프트입니다. 모든 호출은 `transparent_background: false`입니다.

### BoundSunV1 (참조 없음)

```text
Use case: stylized-concept. New full-screen cinematic background for SUNFALL dark gothic fantasy game, landscape 16:9. Wide epic view of a gigantic dark gothic cathedral silhouette in lower third beneath layered storm clouds. High center-right a pale gold sun is imprisoned inside an enormous black iron royal seal and taut black chains stretching diagonally to cathedral spires. The sky around the seal is starting to split into a deep violet cosmic rift, faint crimson edges, sun dimming and falling behind the torn cloud veil. No gold stitches yet. Rich painterly anime game environment illustration, polished atmospheric perspective, desaturated midnight blue, slate, antique gold, restrained crimson. Strong dramatic focal point, large readable shapes, ominous sorrow. Leave lower 22 percent relatively dark for separately rendered subtitles and top 10 percent quiet. Entire image full bleed. No characters, words, UI, title, border, logos or watermarks.
```

### MothersHomeV1 (Hero/HeroPortrait.png를 인물 참조로 사용)

```text
Use case: stylized-concept. Generate a NEW landscape 16:9 cinematic story scene for SUNFALL gothic fantasy. Input image role: identity reference for the young ADULT heroine ONLY, preserve burgundy hair with bangs and low tied bun, pale face, white loose blouse, black long gloves and black waist belt; do not reproduce portrait framing. Scene: a humble medieval cottage interior at night. A compassionate adoptive mother about 50, chestnut hair with grey strands tied back, modest charcoal dress with oatmeal shawl, stands at left by the open wooden door and gently offers warm soup to the young adult heroine at right. Their hands meet near the center as the mother welcomes her home. Intimate maternal warmth, NOT romantic. Warm amber hearth and lantern light, cool moonless blue rain visible through a small gothic window, rough stone and timber. Medium-wide narrative composition with both faces clearly visible in upper-middle area, lower 22 percent mostly dark furniture for subtitles. Polished painterly anime game illustration, restrained detail, deep cinematic shadows, beautiful emotional lighting consistent with a dark gothic game. No text, subtitles, interface, borders, logos, watermark.
```

### SeveredHandsV1 (MothersHomeV1 생성 원본을 인물 참조로 사용)

```text
Use case: stylized-concept. NEW landscape 16:9 dramatic cinematic shot for SUNFALL dark gothic fantasy. Input image is identity and costume reference for the SAME two women only; replace the interior and pose completely. Scene: midnight outside their medieval village home. The older adoptive mother in charcoal dress and oatmeal shawl is being pulled toward a tall crimson-black rift in the upper LEFT sky by royal black iron chains around her forearms. Her worried face remains compassionate. The burgundy-haired adult heroine in white blouse, black long gloves and black belt is in the lower RIGHT foreground reaching desperately toward her. Their open hands are separated by a small heartbreaking gap near CENTER. A single thin golden magical filament starts between their fingertips. Strong sweeping diagonal composition, village roof silhouettes below, storm clouds spiral into the crack, scattered sparks. No injury, blood or gore. Dark navy and violet, restrained red rim light, small gold thread. Polished painterly anime game key art, beautiful emotional faces consistent with reference, wide cinematic framing with enough margins for slow camera movement. Lower 22 percent dark and quiet for separately rendered subtitles. No words, interface, frame, logo or watermark.
```

### GoldenVowV1 (Hero/HeroPortrait.png를 인물 참조로 사용)

```text
Use case: stylized-concept. NEW full-screen landscape 16:9 cinematic final prologue shot for SUNFALL dark gothic fantasy. Input image role: exact identity and costume reference for the young adult heroine ONLY. Preserve burgundy hair with bangs and low tied bun, pale face, white blouse, long black gloves, black waist belt and long dark skirt. Medium-wide shot, heroine at RIGHT third, shown from knees up in three-quarter view facing LEFT toward the distant forest and ruined gothic cathedral. She holds a dark slender tailoring sword lowered in her right hand. Her raised left black-gloved hand at the center cradles fine luminous golden magical thread that winds forward through the air toward the forest at LEFT. Expression quietly determined and grief-stricken. Midnight stone village gate, subtle amber lantern behind her, vast dark blue forest and cathedral in mist at left, torn cloud sky with tiny stars, no sun or daylight. Golden filament is the only hopeful bright accent. High-end painterly anime game illustration, refined face and hands, cinematic blue shadows and antique gold rim light, coherent costume. Leave lower 22 percent relatively dark for subtitles, top 10 percent quiet. No extra people, text, typography, UI, frame, watermark or logo.
```

## 확인 범위

생성 이미지 구성 및 코드·리소스 경로를 확인했습니다. C# 컴파일 로그: `Temp/OpeningCinematicCompile/compile.log`. Unity Play, 게임 QA, 테스트 실행, 플레이어 빌드는 하지 않습니다. 실제 전환과 자막 속도·pause/skip 동작은 본인 QA 대상입니다.
