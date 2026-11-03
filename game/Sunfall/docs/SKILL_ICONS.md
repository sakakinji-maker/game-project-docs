# 고딕 스킬 아이콘 · 2026-10-31

기본 스킬과 1~7층 보스 보상 전체 15종에 고유 아이콘을 추가했습니다. 스킬창의 보유 목록·상단 장착 4칸·상세 설명, 하단 사용 슬롯, 보스 보상 선택에 동일한 아이콘을 사용합니다.

- 장착 스킬: 금빛 테두리, `장착 1~4`, 현재 설정한 단축키 표시.
- 선택 스킬: 밝은 테두리와 상세 패널의 장착 위치 표시. 장착 대상 버튼에도 현재 위치 강조.
- 하단 슬롯: 고유 아이콘, 단축키 명패, 준비/시전/재사용 대기 문구, 충전 막대. 대기 중 아이콘을 어둡게 하고 검은 바탕에 큰 초 단위 숫자를 표시합니다. 마지막 0.1초는 올림해 사용 불가 상태에서 `0.0`으로 표시하지 않습니다.
- 스킬창: 기본/장비 적용/남은 쿨타임과 충전 막대. 창이 열린 동안 시간이 멈추는 기존 동작 유지.
- 기존 습득·장착·입력·전투·쿨타임 계산은 변경하지 않았습니다. 기존 4칸의 터치 입력 영역도 유지합니다.

## 파일과 가져오기

최종 PNG 경로: `Assets/Sunfall/Resources/SkillIcons/<SpellKind>V1.png` (15개).

내장 image_gen 도구로 각각 생성했습니다. CLI/API fallback은 사용하지 않았습니다. 생성 원본을 수정 없이 프로젝트에 복사했습니다. 원본 파일의 루트는 `C:/Users/sakak/.codex/generated_images/01a17569-bfbf-7233-a71a-72eece5bde92/`, 개별 원본 파일명은 `SKILL_ICON_SOURCES.json`의 값에 `exec-`와 `.png`를 붙입니다.

Unity에서는 Default Texture2D / 최대 256px / Bilinear / Clamp / Mipmap 끔 / 무압축으로 가져옵니다. 큰 생성 원본을 작은 UI 크기로 축소하므로 Bilinear를 적용했습니다. 원본 픽셀 파일 자체는 보존됩니다. 새 `SkillIconImporter`와 각 PNG 메타데이터에 같은 설정을 지정했습니다.

`SunfallGame.SkillIcons.cs`는 enum 이름으로 리소스를 한 번 읽어 캐시합니다. 기존 전투 연출용 도형은 유지하며, 이미지가 누락되면 기존 기호로 대체합니다.

## 최종 생성 프롬프트 세트

처음 7개(EmberWave~DawnBurst)의 공통 템플릿:

```text
Use case: stylized-concept. Asset type: ONE square ability icon for SUNFALL, a dark gothic pixel-art action game about sewing a broken sky with golden thread. Subject: {subject} Style: premium crisp chunky pixel art, deliberately bold simple silhouette readable at 32 and 64 pixels, limited palette, dark iron and aged gold accents, strong bright edges. Composition: one centered emblem occupying 75 percent of the square, 12 percent safe padding on every side; no cropping. Background: solid very dark charcoal navy #10121c, opaque. Consistent restrained gothic game inventory art. No enclosing UI frame, no decorative corner filigree, no text, no letters, no digits, no logo, no watermark, no multiple panels, no icon sheet. Square 1:1.
```

나머지 8개의 공통 템플릿:

```text
Use case: stylized-concept. ONE square ability icon for SUNFALL dark gothic pixel-art action game. Subject: {subject} Bold clear simple silhouette readable at 32 and 64 pixels. Crisp chunky pixel art, limited palette, aged gold accents, bright edges. Centered emblem occupies 75 percent, safe margins, opaque dark charcoal navy #10121c background. No UI frame, text, letters, digits, logo or watermark. Square 1:1.
```

MoonBlades는 공통 템플릿의 `aged gold accents, `를 생략했습니다. NightMantle은 `aged gold accents, bright edges` 대신 `silver edges and luminous blue folds`를 사용했습니다. 모든 호출은 `transparent_background: false`, 새 이미지 생성이며 참조 이미지 없음입니다.

| 파일 이름 앞부분 | 표시 이름 | subject |
|---|---|---|
| EmberWave | 불씨 파동 | A horizontal curling wave of orange embers, like a flame sweeping across the ground. Thick bright orange silhouette, ember gold tips. |
| AshLance | 잿불 창 | One long slender spear made of red-orange fire, pointing diagonally up-right, ash-black spearhead core. Clearly a spear, not a sword. |
| CrimsonNova | 홍염 고리 | A complete crimson ring of fire enclosing an empty dark center, short outward flames, red and rose highlights. |
| ChainPull | 포박 사슬 | A hooked dark iron chain forming a strong curved J and pulling inward toward a violet central spark. One prominent large grappling hook, several thick chain links. |
| ChainRain | 사슬 강림 | Three heavy iron chains falling vertically down, each terminating in a sharp spike, violet impact sparks at the bottom. |
| SunLance | 태양의 창 | A slender radiant ivory and warm gold spear pointing straight upward, a small sun halo behind its middle, two narrow rays. Bright gold, no fire-red. |
| DawnBurst | 여명의 폭발 | An explosive golden sunburst expanding in all directions, radiant cream center and eight pointed gold rays. No crown, no weapon. |
| HeartBreaker | 심장 파쇄 | a massive black iron hammer with glowing red heart-shaped crystal inset, striking cracked ground with orange shockwave. |
| CrownGear | 왕관 톱니 | one heavy bronze toothed gear with a jagged royal crown shaped hub and glowing red center, angled flying forward with two short motion trails. |
| MirrorShards | 거울 파편 | three large sharp triangular silver mirror shards spreading in a fan, brilliant lavender-white edges, purple glass facets. |
| MirrorWard | 거울 장막 | one tall pointed gothic mirror shield, reflective silver frame, amethyst glass face, central bright protective glint. Unbroken solid defensive silhouette. |
| MoonBlades | 초승달 쌍날 | two separate opposing icy-blue crescent blades, one on each side facing outward, midnight cobalt core and silver cutting edges. No sun or shield. |
| NightMantle | 밤의 장막 | a dark navy hooded mantle wrapped around a small blue eclipse orb, sweeping cloak silhouette, narrow silver crescent at its heart. |
| RoyalLance | 왕의 광창 | an imposing broad golden royal lance pointing diagonally up-right, black steel center, ornate three-pronged crown crossguard, concentrated gold beam along its edge. |
| SolarCrown | 태양 왕관 | a radiant floating royal crown of five sun rays above a circular gold solar seal, gold-white core, deep burgundy shadows. Clearly a crown and magic circle. |

## 확인 범위

생성 이미지의 내용과 에셋 대응 관계를 확인했습니다. C# 컴파일 로그는 `Temp/SkillIconsCompile/compile.log`입니다. Unity Play·게임 QA·테스트 실행·플레이어 빌드는 하지 않습니다. 실제 화면 배치와 재사용 대기 표시는 사용자가 QA합니다.
