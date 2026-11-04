# SUNFALL Windows QA · 2026-11-05

버전: 0.1.0-qa.20261105.1 / Windows 64비트 / Unity 6000.3.27f1

## 실행

1. `https://github.com/sakakinji-maker/game-project-docs/releases/download/sunfall-qa-2026.11.05-1/SUNFALL-Windows-x64-QA-2026.11.05-1.zip` 에서 ZIP을 받습니다. 저장소 접근 권한이 필요한 경우 GitHub에 로그인합니다.
2. ZIP 전체를 로컬 폴더에 압축 해제합니다. 압축 폴더 안에서 바로 실행하지 않습니다.
3. `Sunfall.exe`를 실행합니다. Unity 에디터 설치는 필요하지 않습니다.
4. `Sunfall_Data`, `MonoBleedingEdge`, DLL 등 동봉한 파일/폴더를 EXE와 함께 유지합니다.

## 조작과 QA 기능

- ←/→ 이동, Ctrl 검, Alt 점프, Shift 구르기
- F/Q/E/R 장착 스킬, I 장비/성장, K 스킬창, Esc 메뉴
- Space NPC/대화, ↑ 문/포탈/상자, ↓+점프 발판 내려가기
- F1~F7: 해당 스테이지 첫 방 이동, 체력·쿨타임 회복, 보유 장비·성장·스킬 유지
- F8: 8스테이지 미구현 안내
- F10: 진행·재화·장비·스토리 시청 기록 초기화. 조작/음량 설정 유지

개발 빌드이므로 개발자 키가 켜져 있고 화면에 Development Build 표시가 나올 수 있습니다.
이 ZIP에는 개인 저장 데이터가 포함되지 않습니다. 다른 PC의 저장 상태는 그 PC에서 관리됩니다.

## 이번 QA 확인 대상 (미실행)

- 스킬 및 어머니 포획·하강 이미지의 잘림
- 하늘 봉합 자국과 5층 거울 봉합의 가시성
- 4층 사슬의 천장 고정 및 카메라 이동 시 연결
- 5층 세 번째 전투방 출구 앞에서 내려온 뒤 ↑로 보스방 진입
- 6층 일식/장막의 범위, 안전지대와 캐릭터 가독성

## 오류 제보

버전, 스테이지/방, 재현 순서, 스크린샷을 함께 기록해 주세요.
Windows 실행 로그: `%USERPROFILE%\AppData\LocalLow\Haeseong\Sunfall Prototype\Player.log`
로그에는 PC 경로 등 환경 정보가 포함될 수 있으니 공유 전 확인하세요.

## 검증

Windows x64 Development 빌드 성공, 오류 0 / 경고 0.
실행 파일과 데이터·DLL 구성, ZIP 무결성을 확인했습니다.
게임 Play/실행, 플레이 QA, 자동 게임 테스트는 수행하지 않았습니다.
이 파일은 QA 배포본이며 정식 출시 버전이 아닙니다.
