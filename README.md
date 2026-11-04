# 정회성 | 게임 기획 · 제작 · QA 포트폴리오

> 플레이 경험을 규칙과 성장 구조로 설계하고, 플레이에서 발견한 문제를 재현 가능한 QA 기록과 개선 요구로 연결합니다.

**[웹게임 플레이](https://onecard-game.onrender.com)** · **[노션 포트폴리오](https://app.notion.com/p/401137cf520a818e95ecd37332cfd544)** · **[QA 사례 모음](qa/README.md)** · **[SUNFALL 문서](sunfall/README.md)** · **[웹게임 상세](web-games/PROJECTS.md)**

이 저장소는 기존 `game-project-docs`(기획 문서·SUNFALL Unity 프로젝트)와 `test-gamesuber`(GAME LAB 웹게임·QA 문서)를 하나로 합친 것입니다. 두 저장소의 커밋 기록은 모두 보존했습니다.

게임 구현은 기획 방향·기능 요구·플레이 피드백을 바탕으로 AI 코딩 도구(Codex 등)와 협업해 진행했습니다. 개인 역할은 기획, 요구사항 정의, 플레이 피드백, QA 기록을 중심으로 설명합니다.

## 프로젝트

| 프로젝트 | 내용 | 자료 |
| --- | --- | --- |
| **SUNFALL — 별을 꿰매는 마녀** | Unity 2D 횡스크롤 액션. 7스테이지·보스 7종·스킬 15종·장비 5종. **PC(키보드) 기준이며 모바일은 아직 대응하지 않았습니다.** | [문서](sunfall/README.md) · [Unity 프로젝트](game/README.md) · [QA](sunfall/06-development-qa.md) |
| **한 수씩 배우는 쇼기** | 12개 튜토리얼 → 7개 전략 수업 → AI·온라인 대국 | [플레이](https://onecard-game.onrender.com/shogi/) · [소스](web-games/public/shogi/) |
| **한 수씩 배우는 바둑** | 12개 입문 문제, 9·13·19줄, 사석 합의 | [플레이](https://onecard-game.onrender.com/baduk/) · [소스](web-games/public/baduk/) |
| **10년 뒤에 나를 찾아줘** | 15챕터·174장면 스토리 미스터리 | [플레이](https://onecard-game.onrender.com/unfinished/) · [기획서](05_아직_끝나지_않은_문장_기획서.md) |
| **원카드** | 2–4인 대전, 온라인 멀티플레이 | [플레이](https://onecard-game.onrender.com/onecard/) · [QA](qa/web-games-qa.md) |
| **핵전쟁 서바이벌** | Canvas 회피·생존 기록 | [플레이](https://onecard-game.onrender.com/survival/) · [기획서](03_뱀파이어서바이벌_기획서.md) |
| **대한민국 생태계교란종 잡기** | 탐험 → 약화 → 포획 → 도감 교육형 RPG | [플레이](https://onecard-game.onrender.com/ecodex/) · [기획서](04_RPG_기획서.md) |
| **애프터라이트: 마지막 도시** | 수집·방치형 RPG (기획 단계) | [기획서](06_애프터라이트_게임기획서.md) |

게임 링크는 배포 경로입니다. 서버 가동 여부와 실제 이용자 성과는 별도로 확인해야 합니다.

## QA 포트폴리오

| 구분 | 내용 | 자료 |
| --- | --- | --- |
| **메이플스토리 월드 QA** | 직접 플레이 메모 53개 → 중복 13개 제거 → 고유 이슈 40개, 우선순위·재검증 절차 정리 | [40개 이슈 보고서](qa/maple-worlds-qa.md) · [CSV](web-games/public/qa/maple-worlds/issues.csv) |
| **원카드 QA** | 개발 중 요청·문제 25건과 처리 결과 | [웹게임 QA](qa/web-games-qa.md) · [원본](qa/onecard-qa.docx) |
| **웹게임 자동화 테스트** | 쇼기·바둑·서사 게임 규칙·UI·멀티플레이 88개 테스트 | [테스트](web-games/tests/) |
| **SUNFALL QA** | 테스트 케이스, 결함 분석·회귀 테스트, 밸런스 검토 | [개발자 기능·QA](sunfall/06-development-qa.md) |

## 폴더 구조

| 경로 | 내용 |
| --- | --- |
| `01_` ~ `06_*.md` | 게임 기획서·대본·설정 |
| [sunfall/](sunfall/) | SUNFALL 정리 문서 |
| [game/](game/) | SUNFALL Unity 프로젝트 (Unity 6000.3.27f1) |
| [web-games/](web-games/) | GAME LAB 웹게임 서버·클라이언트·테스트 (Node.js) |
| [qa/](qa/) | QA 보고서 모음 |
| `render.yaml` | Render 배포 설정 (`web-games` 폴더 기준) |

## 실행

웹게임 (Node.js 22.5.0 이상):

```bash
cd web-games
npm ci
npm start      # http://localhost:3000
npm test
```

SUNFALL은 [game/README.md](game/README.md)의 안내대로 Unity Hub에서 `game/Sunfall` 폴더를 엽니다.

## 문서 수정과 GitHub 반영

로컬 폴더는 `C:\Users\sakak\game-project-docs`입니다.

```powershell
Set-Location 'C:\Users\sakak\game-project-docs'
git pull --ff-only
git status
git add -- <수정한 파일>
git commit -m "변경 내용"
git push
```

문서는 UTF-8과 LF 줄바꿈을 사용합니다.
