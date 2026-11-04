# SUNFALL Unity 게임 프로젝트 (개발 중)

> **개발 진행 중인 프로젝트입니다.** 1~7스테이지까지 구현했고 계속 수정·추가하고 있습니다. 현재 PC(키보드) 기준이며 모바일은 아직 대응하지 않았습니다.

[정리된 게임 문서](../sunfall/README.md) · Unity **6000.3.27f1**

이 폴더에는 실제 편집 가능한 게임 소스·씬·이미지·프로젝트 설정이 들어 있습니다.
이 폴더는 Unity 소스 프로젝트입니다. **[Windows QA 실행 파일 다운로드](https://github.com/sakakinji-maker/game-project-docs/releases/download/sunfall-qa-2026.11.05-1/SUNFALL-Windows-x64-QA-2026.11.05-1.zip)** · [Release](https://github.com/sakakinji-maker/game-project-docs/releases/tag/sunfall-qa-2026.11.05-1) · [실행·QA 안내](WINDOWS_QA.md). 실행 파일과 데이터는 Release ZIP으로 별도 배포합니다.

## 여는 방법

1. 저장소를 복제하거나 Code → Download ZIP으로 내려받아 압축을 풉니다.
2. Unity Hub에서 **6000.3.27f1** 에디터를 설치합니다.
3. Add / 프로젝트 추가에서 이 폴더 아래 **Sunfall**을 선택합니다.
   저장소 루트나 `game` 폴더 자체를 Unity 프로젝트로 선택하지 않습니다.
4. 첫 실행에서 Packages 복원과 Assets 임포트가 끝날 때까지 기다립니다.
5. `Assets/Sunfall/Scenes/Prototype.unity` 씬을 엽니다.
6. Game 창에서 Play로 실행하고 테스트합니다. 최초 프로필은 서장부터 시작합니다.

첫 임포트에는 Unity 패키지를 내려받을 네트워크 연결이 필요할 수 있습니다.
이미지는 일반 Git 파일로 포함되어 별도 LFS 다운로드가 필요하지 않습니다.

## 포함 범위

- `Sunfall/Assets/`: C# 코드, 씬, 모든 프로젝트 에셋과 `.meta` GUID.
- `Sunfall/Packages/`: 패키지 manifest와 lock 파일.
- `Sunfall/ProjectSettings/`: Unity 버전, 빌드 씬 및 입력/프로젝트 설정.
- `Sunfall/CoreTests/`: 독립 코어 검증용 소스.
- `Sunfall/docs/`: 누적 작업 기록. 이전 수치·미술 버전은 현재 코드와 구분합니다.
- `tools/`: 컴파일·공격 아트 측정 스크립트 및 보조 자료.

Library, Temp, Logs, UserSettings, IDE 자동 생성 파일, 빌드 결과와 개인 PlayerPrefs는 제외했습니다.
새 PC에서는 처음부터 시작하며 기존 PC의 골드·장비·스토리 시청 기록을 옮기지 않습니다.

## 기본 조작

←/→ 이동 · Ctrl 검 · Alt 점프 · Shift 구르기 · F/Q/E/R 장착 스킬

Space NPC/대화 · ↑ 문/포탈/상자 · ↓+점프 발판 내려가기 · I 장비/성장 · K 스킬창 · Esc 메뉴

에디터/개발 빌드: F1~F7 해당 층 첫 방, F8 미구현 안내, F10 진행 초기화.

## 최신 수정 및 검증

2026-11-05: 3~7스테이지의 새 배경·보스·등장/처치 연출을 반영했습니다. 스킬/어머니 이미지 잘림, 금빛 봉합, 4층 천장 사슬, 5층 출구 발판과 거울 봉합, 6층 일식·장막 표시를 수정했습니다. [수정 내역과 미실행 QA 항목](Sunfall/docs/RENDERING_STAGE_FIXES_2026_11_05.md).

2스테이지는 방별 2172×724 배경 4장을 사용합니다. 이전 4단 아틀라스의 방별
1086×362 확대를 제거하고 1스테이지와 같은 2070×690 표시 크기로 맞췄습니다.
자세한 생성 기록은 `Sunfall/docs/STAGE2_HD_BACKGROUNDS.md`를 참고하세요.

Core / Runtime / Editor / Tests 컴파일을 확인했습니다. 게임 Play·QA·테스트 실행·EXE 빌드는 수행하지 않았습니다.
`tools/Compile-AttackArt.ps1`은 기존 로컬 Unity Library/Bee 참조가 있는 개발 환경용입니다.
새 체크아웃에서는 먼저 Unity가 패키지와 스크립트를 임포트해 정상 컴파일하는지 확인하세요.
