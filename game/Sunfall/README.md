# 최신 체크아웃 안내

Unity 버전: 6000.3.27f1. 프로젝트 여는 방법은 [상위 게임 안내](../README.md), 최신 정리 문서는 [SUNFALL 문서](../../sunfall/README.md)를 참고하세요.

2026-11-03: 2스테이지는 2172×724 개별 배경 네 장으로 교체했습니다. 상세: [해상도 보정 기록](docs/STAGE2_HD_BACKGROUNDS.md). Core/Runtime/Editor/Tests 컴파일 통과, Play·QA 실행 없음.

아래는 날짜별 누적 작업 기록이며 과거 수치와 이미지를 포함합니다.

2026-11-02 2층 회랑·모르가 개편: 사용자 참고 기반 청록빛 전투방 3개/보스방, 석조 발판, 닫힌/깨진 봉인, 껍질형 모르가 8포즈 및 대화 표정 6종을 적용했습니다. 사슬 절단·반동·파편·껍질 개방·등장 후 대사 연출을 연결했습니다. 상세/에셋/프롬프트: docs/STAGE2_CRYPT_REDESIGN.md. Core·Runtime·Editor·Tests 컴파일 통과: Temp/Stage2CryptCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-02 마을 점프·스킬: 실제 전투 물리·시전·이펙트·쿨타임을 마을에서도 사용합니다. K 스킬 장착, 귀환 시 획득 스킬 연습, NPC/포탈 입력 분리, 고정 카메라를 연결했습니다. 새 모험의 기본 스킬 시작 규칙 유지. 상세: docs/VILLAGE_ACTIONS.md. Core·Runtime·Editor·Tests 컴파일 통과: Temp/VillageActionsCompile/compile.log. 테스트 3개 추가·컴파일만 수행, 플레이·QA·테스트 실행 없음.

2026-11-02 환경설정 개편: 고딕 패널·조작/소리/화면 버튼 탭, 9개 키 버튼과 입력 모달, 음량 게이지, 화면 버튼 표시 저장을 적용했습니다. 키 변경 중 F10의 진행 초기화도 차단했습니다. 상세: docs/SETTINGS_REDESIGN.md. Core·Runtime·Editor·Tests 컴파일 통과: Temp/SettingsRedesignCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-02 시작 화면 개편: 사용자 제공 사슬 태양·대성당 배경, 중앙 금빛 SUNFALL 로고, 한국어 부제, 텍스트 메뉴를 적용했습니다. 화면은 고정하고 작은 금빛 입자만 추가했습니다. 기존 시작/설정 입력과 서장 시청 기록 유지. 상세·이미지·생성 프롬프트: docs/TITLE_REDESIGN.md. Core·Runtime·Editor·Tests 컴파일 통과: Temp/TitleRedesignCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-02 주인공·마을 NPC 표정: 주인공/대장장이/상인/아린 각각 정면 표정 6종, 총 24종을 생성하고 대사 50줄 및 상점·강화 화면에 연결했습니다. 상세·이미지 경로·생성 프롬프트: docs/RESIDENT_DIALOGUE_PORTRAITS.md. Core·Runtime·Editor·Tests 컴파일 통과: Temp/ResidentPortraitCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-02 보스 대화 초상화: 1~7층 정면 상반신·상황별 표정 6종씩 총 42종을 생성하고 등장·도발·처치 대사에 연결했습니다. 상세 및 생성 프롬프트: docs/BOSS_DIALOGUE_PORTRAITS.md. Core·Runtime·Editor·Tests 컴파일 통과: Temp/BossPortraitCompile/compile.log. 시청 기록 유지. 플레이·QA·테스트 실행 없음.

2026-11-02 스토리 시청 기록: 완료/건너뛰기한 서장·보스/층 진입 대사를 즉시 저장하고 다음 실행·새 모험에서는 재생하지 않습니다. 서장을 본 경우 시작 메뉴에서 바로 마을로 갑니다. 구버전 진행 저장은 서장 완료로 이관하며, F10에서 시청 기록도 초기화합니다. 상세: docs/STORY_PROGRESS.md. Core·Runtime·Editor·Tests 컴파일 통과: Temp/StoryProgressCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-02 코드 점검 수정: 공중 공격의 착지 후 상태 잔류·회수 방향 불일치, 스킬 슬롯 터치 누락, 미니맵/메뉴 버튼 겹침, 입력 대기 중 이펙트 시간 진행을 수정했습니다. 고딕 패널 중앙부 그리기 호출도 줄였습니다. 상세: docs/REVIEW_FIXES_2026_11_02.md. Core·Runtime·Editor·Tests 컴파일 통과: Temp/ReviewFixCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-02 스킬 애니메이션 및 시작 화면 수정: 첫 화면에서 추가 GUI 그룹을 제거하고 기존 캔버스 좌표 방식으로 복원했습니다. 불꽃 8프레임 루프·불씨 흐름·사슬 마디의 발사/휘어짐/반동·고리/보호막 입자·광창/파편 잔상을 적용했습니다. 상세: docs/SPELL_ANIMATION.md. 이미지: Assets/Sunfall/Resources/Effects/FlameLoopV1.png. 생성 프롬프트: docs/FLAME_ANIMATION_PROMPT.md. 컴파일 로그: Temp/SpellAnimationCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-02 화면 표현 통합 수정: 스킬 15종 실제 이미지 이펙트, 장비 5종 이미지(인벤토리·장착 칸·상점·강화·보상), 1~7층 보스 대화 초상화를 연결했습니다. 공중 베기 배율을 사용자 피드백에 따라 0.21로 재조정하고 화면 클리핑 및 화면 밖 일반 몬스터 공격 대기를 적용했습니다. 상세: docs/PRESENTATION_FIXES.md. 생성 프롬프트: docs/PRESENTATION_ASSET_PROMPTS.md. Core·Runtime·Editor·Tests 컴파일 통과: Temp/PresentationFixCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-02 점프 공격 크기 수정: 공중 베기의 표시 배율을 0.24에서 0.18로 줄이고, 준비·베기·회수 8개 프레임에 동일하게 적용했습니다. 골반 기준 위치와 기존 공격 판정은 유지합니다. 상세: docs/ATTACK_FRAMES.md. Core·Runtime·Editor·Tests 컴파일 통과. 로그: Temp/AirAttackScaleCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-01 캐릭터 반짝임 수정: 프레임 두 장을 반투명하게 겹치며 몸통 불투명도가 주기적으로 떨어지던 처리를 제거했습니다. 주인공·마을 NPC·일반 몬스터·전체 보스를 현재 프레임 한 장으로 표시합니다. 위치 보간·공격 중간 프레임·발 기준 정렬·피격/사망 효과는 유지합니다. 상세: docs/CHARACTER_FLICKER_FIX.md. Core·Runtime·Editor·Tests·CoreChecks 컴파일 통과. 로그: Temp/CharacterFlickerCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-01 서장 화면 고정 및 개발자 초기화: 서장 카메라의 좌우 이동·확대·GUI 그룹 변환을 제거했습니다. 에디터/개발 빌드에서 F10을 누르면 골드·장비·강화·성장·현재 모험을 초기화하고 시작 화면으로 복귀합니다. 키·음량 설정 및 보스 처치 자동 저장은 유지합니다. 상세: docs/DEVELOPER_RESET.md. 컴파일 로그: Temp/StableOpeningDeveloperResetCompile/compile.log. Core·Runtime·Editor·Tests·CoreChecks와 일반/개발 빌드 조건부 코드 컴파일 통과. 실제 초기화·플레이·QA·테스트 실행 없음.

2026-11-01 서장 시네마틱: 이야기 시작을 전체 화면 2.5D 애니메이션으로 변경했습니다. 전용 일러스트 4장과 마을 배경을 사용해 9개 대사에 카메라 이동·사슬·불빛·입자·별실·자동 자막·암전 전환을 연결했습니다. Space/Enter 자막 펼치기·다음, Esc 일시정지, 전체 건너뛰기 및 종료 후 마을 연결을 지원합니다. 상세/생성 프롬프트: docs/OPENING_CINEMATIC.md. 최종 이미지: Assets/Sunfall/Resources/Story/Cinematic/. Core·Runtime·Editor·Tests·CoreChecks 컴파일 통과. 로그: Temp/OpeningCinematicCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-01 대사창 하늘 일러스트: 임시 봉합 선 그림을 구름·깊은 균열·금빛 실·고딕 첨탑이 있는 전용 일러스트로 교체했습니다. 비율을 유지하고 자막 영역을 분리했습니다. 최종 이미지: Assets/Sunfall/Resources/Story/StitchedSkyV1.png. 생성 프롬프트와 적용 범위: docs/STORY_SKY_ART.md. Core·Runtime·Editor·Tests·CoreChecks 컴파일 통과. 로그: Temp/StorySkyArtCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-11-01 시작 화면: SUNFALL — 별을 꿰매는 마녀를 정식 타이틀로 표시합니다. 기존 고딕 마을 배경과 주인공 일러스트, 금빛 봉합선·입자 연출, 세로형 시작/환경 설정 메뉴를 적용했습니다. 마우스·터치와 ↑↓/Enter/Space로 메뉴를 선택합니다. 상세: docs/TITLE_SCREEN.md. Core·Runtime·Editor·Tests·CoreChecks 컴파일 통과. 로그: Temp/TitleScreenCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-10-31 고딕 스킬 아이콘: 기본 스킬과 1~7층 보상 전체 15종 전용 아이콘을 스킬창·하단 장착 슬롯·보스 보상 선택 화면에 적용했습니다. 장착 칸과 단축키, 대기 중 큰 쿨타임 숫자, 충전 막대와 준비/시전/대기 문구를 추가했습니다. 상세 및 생성 프롬프트: docs/SKILL_ICONS.md. 최종 PNG: Assets/Sunfall/Resources/SkillIcons/. Core·Runtime·Editor·Tests·CoreChecks 컴파일 통과. 로그: Temp/SkillIconsCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-10-31 스킬 장착: 모험 중 K 또는 상단 스킬 버튼으로 설명·기본/장비 적용/남은 쿨타임을 확인하고 최대 4개를 장착·해제합니다. 기본 F/Q/E/R은 스킬 이름이 아닌 슬롯 1~4를 사용합니다. 미장착 스킬은 보유 목록에 남고 발동하지 않으며, 교체나 슬롯 이동으로 쿨타임을 초기화할 수 없습니다. 새 스킬은 빈 슬롯에만 자동 장착하고, 가득 찼으면 선택 후 스킬창을 엽니다. 상세: docs/SKILL_LOADOUT.md. 컴파일 로그: Temp/SkillLoadoutCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-10-31 전 층 스킬 선택: 1~3층도 4~7층과 동일하게 처치 대사·연출 후 두 스킬 중 하나를 직접 고릅니다. 이전에 획득한 모든 스킬을 유지하며, 선택 전에는 다음 방/상자 진행을 보류합니다. 선택창에는 스킬 설명과 실제 지정 키를 표시하고 운명에 맡기기 버튼도 유지했습니다. 상세: docs/BOSS_SKILL_CHOICE.md. 컴파일 로그: Temp/AllBossSkillChoiceCompile/compile.log. 플레이·QA·테스트 실행 없음.

2026-10-31 후반 보스 스킬: 4~7층 보스마다 2개 중 1개를 직접 선택합니다. 선택 화면의 운명에 맡기기를 누르면 50% 무작위로 얻습니다. 심장 파쇄/왕관 톱니(Y), 거울 파편/거울 장막(U), 초승달 쌍날/밤의 장막(O), 왕의 광창/태양 왕관(P)을 추가했습니다. 모든 기존 보상과 새 보상은 같은 모험에서 유지되며, 키 중복을 방지하고 개별 재사용 대기시간과 HUD·터치 입력을 연결했습니다. 상세: docs/LATE_BOSS_SKILLS.md. 검증은 컴파일만, 플레이·QA·테스트 실행 없음.

2026-10-31 후반부 확장: 4층 바르칸 → 5층 거울 성역/리세아 → 6층 일식의 첨탑/녹티스 → 7층 꺼진 태양의 왕좌/솔베인. 각 층 전투방 3개와 보스방, 전용 도트·3종 공격·체력 절반 2페이즈·등장/처치 대사·상자·순차 포탈을 연결했습니다. 획득한 모든 스킬, HP, 장비와 성장은 유지됩니다. 7층 처치 후 양어머니 구출과 태양 복귀 연출, 마지막 포탈에서 결말로 이어집니다. 상세: docs/SEVEN_CHAPTER_CAMPAIGN.md. 검증은 컴파일만 수행하며 플레이·QA·테스트 실행은 하지 않습니다.

> 최신 변경(2026-10-27): 사용자 참고 이미지로 재생성한 고딕 도트 배경을 불씨 마을에 적용했습니다. 대장간·등불 쉼터·상점·성문과 기존 NPC를 연결했습니다. 생성 프롬프트와 경로는 `docs/VILLAGE_BACKGROUND.md`, 컴파일 로그는 `Temp/VillageBackgroundCompile/compile.log`입니다. 플레이·QA·테스트 실행은 하지 않았습니다.

> 1탄은 숲 입구 → 무너진 정원 → 악마 주둔지 → 공중 보스 성소의 네 방입니다. 배치와 진행 규칙은 `docs/CHAPTER_ONE_ROOMS.md`를 참고하세요.

# SUNFALL — 별을 꿰매는 마녀

2026-10-30 4스테이지: 아우렐 뒤 포탈을 왕관의 심장부로 연결했습니다. 왕관의 외벽(8) → 역광의 도관(9) → 심장 봉인실(10), 금속 발판과 태양 도관·왕관 심장 장식, 진입 대사 4쪽을 추가했습니다. 모든 획득 스킬과 HP·장비·성장을 유지합니다. 이후 4층 보스와 5~7층을 추가했으며 현재는 바르칸 처치 후 5층으로 이어집니다. 상세: docs/CHAPTER_FOUR.md. 컴파일 로그: Temp/ChapterFourCompile/compile.log. 플레이·QA·테스트 실행은 하지 않았습니다.

2026-10-30 태양 스킬 보상: 아우렐 처치 시 태양의 창(관통) / 여명의 폭발(주변 광역) 중 하나를 50% 확률로 획득합니다. 별도 T 키로 사용하고 이전 보상과 기본 스킬을 모두 유지합니다. 키 중복 방지, HUD·터치 슬롯·획득 안내·결과 표시를 연결했습니다. 상세: docs/SOLAR_SKILLS.md. 컴파일 로그: Temp/SolarSkillsCompile/compile.log. 플레이·QA·테스트 실행은 하지 않았습니다.

2026-10-30 3스테이지 보스: 태양 봉인의 내전과 집행관 아우렐을 추가했습니다. HP 600, 태양 베기·왕실 광창·흑일 심판, 절반 체력 2페이즈, 양어머니의 목소리로 이어지는 등장 6쪽/처치 7쪽 대사, 봉인 해방 연출과 보물상자를 연결했습니다. 기존 획득 스킬을 모두 유지합니다. 상세와 이미지 생성 기록: docs/STAGE3_SOLAR_REGENT.md. 컴파일 로그: Temp/SolarBossCompile/compile.log. 플레이·QA·테스트 실행은 하지 않았습니다.

2026-10-30 3스테이지 연결: 모르가 처치 뒤 포탈에서 ↑를 누르면 왕도 대성당으로 이동합니다. 진입로(7) → 왕실 회랑(8) → 태양 봉인 예배당(9)의 전투방 3개, 석조 발판과 고딕 실내 장식, 진입 대사 3쪽을 추가했습니다. 두 보스에게 얻은 스킬과 현재 HP·장비·성장을 유지합니다. 이후 예배당 다음 방에 아우렐 보스를 추가했습니다. 보스 처치 뒤 귀환 포탈로 탐색을 마칩니다. 상세: docs/CHAPTER_THREE.md. 컴파일 로그: Temp/ChapterThreeCompile/compile.log.

2026-10-30 사슬 스킬 보상: 2스테이지 모르가 처치 시 포박 사슬(전방 적 끌어오기) / 사슬 강림(주변 광역 내려치기) 중 하나를 50% 확률로 자동 획득합니다. 별도 R 키로 사용하며 기존 1스테이지 보상과 불씨 파동은 유지합니다. 상세: docs/CHAIN_SKILLS.md. 컴파일 로그: Temp/ChainSkillsCompile/compile.log. 플레이와 QA는 하지 않았습니다.

2026-10-30 2스테이지 보스: 왕실 사슬의 제단과 간수 모르가를 추가했습니다. HP 420, 사슬 내려치기·구속 사슬·사슬 낙하, 절반 체력 2페이즈, 등장/처치 대사 각 6쪽과 사슬 붕괴 연출을 연결했습니다. 상세 및 이미지 생성 기록: docs/STAGE2_CHAIN_WARDEN.md. 컴파일 로그: Temp/ChainWardenCompile/compile.log. 플레이와 테스트는 실행하지 않았습니다.

2026-10-30 하늘 봉합 연출: 1스테이지 성소 하늘에 붉은 균열을 추가했습니다. 보스 처치 대사 후 주인공의 금빛 별실이 올라가 11개의 교차 바늘땀으로 균열을 닫고, 성소를 떠날 때까지 봉합 자국을 남깁니다. 상세: `docs/SKY_SEAM.md`. 컴파일 로그: `Temp/SkySeamCompile/compile.log`.

2026-10-30 이야기 적용: 양어머니가 균열에 끌려간 밤을 다룬 시작 대사 9쪽, 아린 6쪽·대장장이 5쪽·상인 5쪽 대화, 1스테이지 보스 등장 5쪽·처치 7쪽을 적용했습니다. 제목 화면의 이야기 시작으로 서장을 볼 수 있습니다. Space / 화면 버튼으로 진행하고, 보스 대화 중에는 연출과 전투가 멈춥니다. 상세와 전체 대본: `docs/STORY_DIALOGUE.md`. 컴파일 로그: `Temp/StoryDialogueCompile/compile.log`.

2026-10-28 2스테이지 연결: 1스테이지 보스 뒤 포탈에서 ↑를 누르면 잿빛 성문 → 쇠사슬 회랑 → 검은 성채로 진행합니다. 보스 보상 스킬, 현재 HP, 장비와 성장을 유지합니다. 2스테이지 전투방 3개를 구현했으며 이후 왕실 사슬의 제단과 보스 모르가를 추가했습니다. 상세: `docs/CHAPTER_TWO.md`. 컴파일 로그: `Temp/ChapterTwoCompile/compile.log`.

2026-10-28 보스 스킬 획득 수정: 새 모험은 불씨 파동만 보유하며 잿불 창·홍염 고리는 잠겨 있습니다. 1탄 보스 처치 시 두 스킬 중 하나를 시드 기반 무작위로 자동 획득합니다(상자와 별도). 획득은 해당 모험에만 유지하며 새 도전/같은 시드 재도전에는 다시 잠깁니다. 잠긴 스킬은 키보드·터치 모두 발동하지 않습니다. 컴파일 로그: `Temp/BossSkillUnlockCompile/compile.log`.

2026-10-28 마을 경제·영구 성장: 몬스터 처치 골드/경험치 자동 획득, 상인 장비 구매, 대장장이 +5 강화, I → 능력치에서 공격력·마력·체력·방어력·민첩 배분을 추가했습니다. 시작 2 P, 레벨업마다 2 P이며 마을에서 배분합니다. 결과 화면과 일시정지 메뉴에서 마을로 귀환할 수 있습니다. 상세: `docs/TOWN_PROGRESSION.md`. 컴파일만 확인했으며 로그는 `Temp/TownProgressionCompile/compile.log`입니다.

2026-10-28 장비 변경: 장비 5종과 I 인벤토리, 안전 구간 장착·해제, 보스 상자의 미보유 장비 1개 지급, 획득·장착 상태 저장을 구현했습니다. 상세: `docs/EQUIPMENT_PROPOSAL.md`. 컴파일만 확인했으며 로그는 `Temp/EquipmentInventoryCompile/compile.log`입니다.

2026-10-28 처치·보상·마법 변경: 보스의 붉은 입자 소멸 → 포탈 개방 → 상자 등장 연출을 추가했습니다. 상자 앞 ↑로 회복 물약을 즉시 사용합니다(최대 HP 40 회복, 전투 중 획득 불가). Q 잿불 창 / E 홍염 고리를 추가했습니다. 상세: `docs/BOSS_REWARDS_SKILLS.md`.

2026-10-28 보스 등장씬: 보스방 진입 시 중앙 마법진에서 수호자가 나타나는 3.6초 연출을 추가했습니다. 연출 중 전투를 잠시 멈추고 카메라가 주인공에게 돌아오면 조작을 재개합니다. 상세: `docs/BOSS_INTRO.md`.

2026-10-28 UI 변경: 플레이어·보스 체력바와 하단 스킬 표시를 고딕 금속 장식과 원형 문장으로 교체했습니다. HP, 위험 상태, 재사용 대기시간, 키 설정을 표시합니다. 상세: `docs/GOTHIC_HUD.md`. 컴파일만 확인했으며 로그는 `Temp/GothicHudCompile/compile.log`입니다.

Unity / C# 2D 횡스크롤 싱글 액션 프로토타입입니다. 게임명은 작업용 이름입니다. 현재 버전은 **증강 없이 1~7스테이지와 일곱 보스, 양어머니 구출 결말**을 구현합니다.

## 현재 플레이 흐름

마을 정비 → 숲 입구(근접병 4) → 무너진 정원(근접병 4·궁수 2) → 악마 주둔지(근접병 3·궁수 2·중갑병 2) → 공중 성소 비행 보스 → 보상 획득 → 2스테이지 잿빛 성문(6) → 쇠사슬 회랑(7) → 검은 성채(8) → 왕실 사슬의 제단(모르가) → 보상·왕도 포탈 → 3스테이지 대성당 진입로(7) → 왕실 회랑(8) → 태양 봉인 예배당(9) → 태양 봉인의 내전(아우렐) → 보상·심장부 포탈 → 왕관의 외벽(8) → 역광의 도관(9) → 심장 봉인실(10) → 탐색 완료 화면.

각 방은 가로 64 단위입니다. 현재 방의 적을 모두 잡아야 다음 문이 열리며, 문은 점프·구르기로 넘어갈 수 없습니다. 지상에서 열린 문 앞으로 이동해 ↑를 누르면 남은 체력을 유지한 채 다음 방에 진입합니다. 마을 출발과 보스 처치 후 2스테이지 포탈도 같은 방식입니다. 발판은 1스테이지 3/5/5/6개, 2스테이지 4/5/5/4개, 3스테이지 5/5/6/4개, 4스테이지 5/6/7개입니다. 보스방 중앙은 비행 공격을 읽을 수 있도록 비워두었습니다.

**증강 선택과 방 클리어 보상 화면은 없습니다.** 영구 능력치는 처치 경험치로 얻은 포인트를 마을에서 배분합니다.

## 구현 내용

- 태양이 사라진 세계의 마을, 대장장이·상인·아린 도트 NPC, 개별 대화와 주인공 답변 초상화.
- 붉은 묶음머리·흰 블라우스·검은 긴 장갑의 주인공 도트.
- 검 3연타(100% / 100% / 160%), 공중 베기, 단일 점프.
- 구르기: 공격·마법 시전 취소, 시작 0.2초 무적, 재사용 0.7초.
- 불씨 파동: 0.2초 시전, 관통·250% 피해, 재사용 5초, 지상·공중 사용.
- 1탄 비행 보스 붉은 날개의 수호자: 지면 파동, 대각선 돌진, 붉은 마법탄. HP 50% 이하에서 2페이즈 전환.
- 보스 날갯짓·시전·돌진 도트, 공격 경로 예고, 페이즈 체력바, 높이를 반영한 공격 판정. 일반 악마 3종은 숲 전투장에 등장합니다.
- 새 공중 성소 배경과 이끼 낀 석재 발판. 장식 배경과 실제 충돌 발판은 별개입니다.
- 사망/완료 화면, 같은 시드·새 시드 재도전, 키 설정·음량 저장, 포커스 상실 시 정지.
- 모바일 다중 터치, 안전 영역, 화면 비율 대응, PC 터치 미리보기.

## 실행

1. Unity Hub에서 이 폴더의 `Assets`, `Packages`, `ProjectSettings`가 있는 프로젝트를 엽니다.
2. 현재 컴파일 확인에 사용한 설치 버전은 **6000.3.27f1**입니다. 원래 프로젝트 기준 버전은 6000.0.58f2입니다.
3. Unity의 **Sunfall → Open Prototype**으로 씬을 엽니다. 사용자 QA 시 Play를 누르고 여정을 새로 시작합니다.
4. 게임은 실행 시 구성됩니다. 에디터 메뉴의 **Sunfall → Build Windows Prototype**으로 플레이어 빌드를 만들 수 있으며 해당 플랫폼 지원 모듈이 필요합니다.

## 기본 조작

| 행동 | PC |
|---|---|
| 이동 | ← / → |
| 검 공격 | Ctrl 좌우 모두, 유지하면 3연타 / 공중에서는 공중 베기 |
| 점프 | Alt 좌우 모두 |
| 발판 내려가기 | ↓를 누른 채 점프 키 (기본 Alt). 현재 발판을 통과해 아래 발판 또는 지면에 착지 |
| 마을 NPC 상호작용 | 가까이에서 Space / 이름표 클릭. 상인은 구매, 대장장이는 강화, 아린은 대화. 상점의 이야기 듣기로 기존 대화도 가능 |
| 문 이동 | 문 앞에서 ↑. 전투방은 적 전원 처치 후 지상에서 사용. 터치 조작에서는 문 앞에 표시되는 이동 버튼 사용 |
| 구르기 | Shift 좌우 모두, 지상에서 누를 때 사용 |
| 불씨 파동 | F |
| 잿불 창 | 1탄 보스 무작위 보상으로 획득 후 Q (고속 관통, 기본 재사용 6초) |
| 홍염 고리 | 1탄 보스 무작위 보상으로 획득 후 E (주변 원형 공격, 기본 재사용 8초) |
| 보물상자 | 보스 처치 연출 후 가까이에서 ↑. 물약 즉시 사용 + 미보유 장비 1개 획득 |
| 인벤토리 / 성장 | I / 화면 버튼. 장착·해제는 마을 또는 적을 모두 처치한 방, 능력치 포인트 배분은 마을에서 가능 |
| 정지 / 취소 | Esc |

설정 화면에서 키를 변경할 수 있습니다. 기존 사용자 키 설정은 유지합니다. 모바일은 화면 버튼을 사용합니다.

## 검증과 한계

2026-10-26 공격 중간 프레임 변경: 주인공 5종, 일반 악마 3종, 보스 3종의 공격을 각 8프레임으로 연결했습니다. Core / Runtime / Editor / Tests **컴파일만 성공**했으며 컴파일러 오류·경고 출력은 없습니다. 로그는 `Temp/AttackFramesCompile/compile.log`, 아트·타이밍·프롬프트는 `docs/ATTACK_FRAMES.md`입니다. Play 모드와 테스트 실행은 하지 않았습니다.

2026-10-26 비행 보스·패턴·전용 맵 변경: Unity 6000.3.27f1의 Roslyn과 프로젝트 Bee 참조 설정으로 Core / Runtime / Editor / Tests 및 독립 CoreTests 프로그램을 **컴파일만 확인**했습니다. 로그는 `Temp/AerialBossCompile/compile.log`입니다.

사용자 요청에 따라 Play 모드, 자동 테스트 실행, 실제 게임 QA, 플랫폼 빌드는 실행하지 않았습니다. 이전 버전에서 기록한 테스트 통과는 이번 맵·전투 변경의 결과가 아닙니다. 난이도, 카메라 이동감, 발판 동선, 공격 가독성은 사용자 QA 후 조정합니다.

1탄 보스 아트 변경도 Core / Runtime / Editor / Tests 및 CoreChecks 컴파일을 확인했습니다. 로그는 `Temp/BossCompile/compile.log`이며 플레이와 테스트 실행은 하지 않았습니다.

현재는 IMGUI로 그리는 프로토타입입니다. 캐릭터 공격은 동작별 8프레임 도트를 사용하며 대기·이동은 기존 아트를 사용합니다. 골드·경험치·장비·강화·능력치는 저장하지만 런 저장/이어하기, 컨트롤러, 모바일 버튼 재배치는 없습니다. 완성된 exe/apk나 스토어 배포판이 아닙니다.

## 코드 및 문서

- `Assets/Sunfall/Runtime/SunfallGame.AttackArt.cs`: 공격 11종, 88프레임의 원본 UV·기준점·공격 타이밍 연결.
- `docs/ATTACK_FRAMES.md`: 최신 공격 도트, 저장 경로, 생성 프롬프트, 컴파일 범위.
- `Assets/Sunfall/Core/RunModel.cs`: 고정 시간 시뮬레이션, 연속 맵·적 생성, 전투, 출구 판정.
- `Assets/Sunfall/Runtime/SunfallGame.cs`: 화면·입력·게임 흐름.
- `Assets/Sunfall/Runtime/SunfallGame.Map.cs`: 지도 아트, 카메라, 미니맵, 출구 표시.
- `Assets/Sunfall/Runtime/SunfallGame.Village.cs`: 마을과 NPC 대화.
- `Assets/Sunfall/Runtime/SunfallGame.Hero.cs`, `SunfallGame.SwordArt.cs`: 주인공 아트와 검 동작.
- `Assets/Sunfall/Runtime/SunfallGame.Demons.cs`: 악마 아트·예고·화살 표시.
- `Assets/Sunfall/Runtime/SunfallGame.Boss.cs`: 1탄 보스 도트·공격 예고·충격파·체력바.
- `docs/STAGE1_BOSS.md`: 보스 디자인, 시트·프롬프트, 전투 수치, 컴파일 범위.
- `Assets/Sunfall/Resources/World/`: 숲 파노라마와 지형 PNG.
- `docs/PROGRESSION_MOTION.md`: 현재 일반 전투 → 보스전 순서, 문 제한, 이동·자세 보간, 컴파일 범위.
- `Assets/Sunfall/Runtime/SunfallGame.Motion.cs`: 캐릭터·투사체 위치 보간과 자세 전환.
- `docs/AERIAL_BOSS.md`: 비행·패턴·2페이즈 수치와 아트 프롬프트. 보스 직행 설명은 후속 변경으로 대체되었습니다.
- `docs/WORLD_MAP.md`: 이전 숲 맵·증강 제거 작업 기록.
- `docs/DEMONS.md`, `docs/CHARACTER_ART.md`, `docs/COMBAT_ART_V2.md`: 이전 아트 제작 기록. 방 구성 등 과거 구현 설명은 현재 진행 순서는 `docs/PROGRESSION_MOTION.md`를 우선합니다.
- `docs/COMBAT.md`, `docs/DECISIONS.md`, `docs/QA.md`: 이전 전투·기획·QA 참고 기록.
- `CoreTests/`, `Assets/Sunfall/Tests/EditMode/`: 회귀 테스트 소스. 보상 관련 검증은 새 탐험·출구 구조에 맞춰 갱신했습니다.

테스트를 직접 실행하려면 .NET 8 환경에서 `dotnet run --project CoreTests/CoreTests.csproj`, 또는 Unity Test Runner의 EditMode를 사용할 수 있습니다. 이번 작업에서는 실행하지 않았습니다.

## 폰트

내장 한글 폰트는 SIL OFL 1.1의 Noto Sans CJK KR을 부분 집합으로 만든 `Sunfall Korean Subset`입니다. 저작권과 라이선스는 `Assets/Sunfall/Resources/OFL.txt`를 참고하세요. 실행 환경에 설치된 맑은 고딕, Apple SD Gothic Neo, Noto Sans CJK KR을 우선 사용하며, 해당 OS 폰트가 없는 기기에서는 새 대사 글리프를 내장 폰트에 추가해야 합니다.



