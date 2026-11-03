> 2026-10-31 변경: 1~7층 모두 처치 연출 후 두 스킬 중 하나를 직접 선택합니다. 자동 지급은 하지 않으며, 운명에 맡기기를 눌렀을 때만 무작위 획득합니다. 아래 자동/무작위 지급 설명은 이전 구현 기록입니다. 현재 규칙: BOSS_SKILL_CHOICE.md.

> 후속 수정: 잿불 창과 홍염 고리는 기본 잠금이며 1탄 보스 처치 시 둘 중 하나를 이번 모험용으로 자동 획득합니다. 상세는 BOSS_SKILL_UNLOCK.md를 참고하세요.

# 보스 처치 연출·보물상자·추가 마법 (2026-10-28)

## 처치 및 보상

- 보스 HP가 0이 되면 3.2초 처치 연출을 시작합니다. 카메라가 처치 위치를 비추고 보스가 붉게 물들어 사라지면서 붉은 입자가 위로 흩어집니다.
- 연출 시작 약 2초 뒤 귀환 포탈이 열리고, 약 2.5초 뒤 보스가 쓰러진 위치 아래 지면에 보물상자가 나타납니다. 상자 X는 맵 안쪽 8~56 범위에 고정합니다.
- 연출 중 주인공 조작·피해·보스 공격을 막고 투사체를 정리합니다. 종료하면 카메라와 조작이 돌아옵니다. Esc·포커스 상실 시 연출도 정지합니다.
- 지상에서 상자 2단위 이내에 접근한 뒤 ↑로 열 수 있습니다. 터치 화면에서도 상자 근처에 열기 버튼이 표시됩니다.
- 상자에는 회복 물약 1개가 있으며 열자마자 자동 사용됩니다. 최대 40 HP를 즉시 회복하고 최대 HP를 넘지 않습니다. 이미 체력이 가득한 경우에도 물약은 사용 완료로 처리합니다.
- 물약 보관·전투 중 사용 키는 없습니다. 살아 있는 적이 있거나 처치 연출 중에는 상자를 열 수 없습니다. 한 상자에서 회복은 한 번뿐입니다.
- 상자를 열지 않고 귀환 포탈로 나가는 것도 가능합니다. 후속 변경으로 미보유 장비 1개 드롭과 인벤토리가 추가되었습니다(`EQUIPMENT_PROPOSAL.md`). 마을 구매·강화·영구 성장은 `TOWN_PROGRESSION.md`를 참고하세요.

## 추가 스킬

| 스킬 | 기본 키 | 효과 | 재사용 |
|---|---|---|---|
| 불씨 파동 (기존) | F | 전방 7.5단위 관통, 공격력 250% | 5초 |
| 잿불 창 | Q | 전방 12단위 고속 관통, 공격력 320%, 각 적 1회 피해 | 6초 |
| 홍염 고리 | E | 자신 중심 반경 3.6단위 원형 공격, 공격력 200%, 양쪽·공중 적에 각 1회 피해 | 8초 |

- 세 마법은 독립 재사용 시간을 가집니다. 기존 0.2초 시전과 0.18초 후딜 및 마법 시전 도트를 공유합니다. 각 스킬의 투사체·범위 효과와 HUD 아이콘은 구분했습니다.
- 새 마법도 구르기로 시전을 취소할 수 있습니다. 취소하면 효과나 재사용 시간이 발생하지 않습니다.
- 동시에 여러 마법을 누르면 사용할 수 있는 불씨 파동 → 잿불 창 → 홍염 고리 순서로 하나를 시전합니다. 한 번 누를 때 한 번 시전하며 유지로 연속 시전하지 않습니다.
- 기존 저장 키 6개는 유지하고 새 키를 추가했습니다. 기본 Q/E가 기존 저장 키와 겹치면 비어 있는 키를 선택하며, 설정 화면에서 실제 키를 확인·변경할 수 있습니다.
- 하단 문장 UI에 새 스킬·쿨다운을 추가하고, 터치 버튼과 상자/문 버튼이 겹치지 않도록 위치를 조정했습니다. 키 설정은 8개 항목을 표시합니다.

## 에셋

내장 image_gen으로 생성했으며 CLI/API fallback은 사용하지 않았습니다. 생성 원본을 프로젝트에 복사하고 UV로 상태를 선택합니다.

- `Assets/Sunfall/Resources/World/RewardChestV1.png`: 1774×887, 닫힘·열림 상태, 실제 알파 투명 배경.
- `Assets/Sunfall/Resources/World/HealingPotionV1.png`: 1254×1254, 즉시 회복 알림용 물약 아이콘, 실제 알파 투명 배경.
- 임포트: 기존 WorldArtImporter의 Point / Clamp / 무압축 / 밉맵 없음 / NPOT 유지 설정.

## 확인 범위

Core / Runtime / Editor / Tests 및 독립 CoreChecks 컴파일만 확인합니다. 로그: `Temp/BossRewardsSkillsCompile/compile.log`.
처치 순서·전투 차단·회복 상한·반복 획득 방지·전투 중 상자 제한·관통·광역 피해·독립 쿨다운·시전 취소 테스트 소스를 추가했습니다. 테스트 실행, Unity Play, 실제 플레이·시각 QA는 하지 않습니다.

## 생성 프롬프트

### 상자
```text
Use case: stylized-concept. Production pixel-art treasure chest sprite sheet for SUNFALL, a sunless gothic medieval side-scrolling action game. Exactly TWO equal cells side by side in one horizontal row. Left: CLOSED squat treasure chest, dark aged walnut wood, dark slate iron bands with slender gothic pointed filigree, aged brass corners, central small crimson gemstone lock. Right: the EXACT same chest OPEN, lid raised, empty dark interior with subtle warm amber magical light, same base position, scale, and silhouette width. Near frontal orthographic side-scroller prop with only a small visible top surface, full object, no floor or shadow outside object. Crisp handcrafted pixel clusters, readable when reduced to 75x60 px. Both complete sprites centered with matching bottom baseline and large transparent margins. No items emerging from chest, no coins, people, lettering, labels, scenery, watermarks. Actual transparent alpha, no painted checkerboard. Wide landscape sheet.
```

### 물약
```text
Use case: stylized-concept. ONE isolated pixel-art healing potion inventory icon for SUNFALL gothic medieval fantasy game. A squat faceted ruby-red glass flask full of warm glowing red liquid, tiny aged brass neck ornament shaped like a sun, dark cork, small cream reflection. Clear simple silhouette readable at 32x40 pixels, handcrafted crisp pixel clusters, dark outline, no text, no letters, no border, no scenery, no people, no other objects, no background or cast shadow. Centered complete bottle with generous transparent margins on square canvas, actual transparent alpha, no painted checkerboard.
```
