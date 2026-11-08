# 게임 QA 포트폴리오

[포트폴리오 홈](../README.md) · [프로젝트 상세](../web-games/PROJECTS.md)

## 게임하면서 찾은 버그들

출시된 게임을 플레이하다 발견한 버그를 QA 리포트 형식으로 기록합니다.

- [버그 목록](game-bug-reports/README.md)
- [001 쿠키런: 킹덤 — 퀘스트 보상 수령 후 오류 반복](game-bug-reports/001-cookierun-kingdom-quest-reward-error.md)

## SUNFALL (Unity)

PC 기준 테스트 케이스 14개, 결함 리포트 3건, 회귀 테스트, 장비 밸런스 분석을 정리했습니다. 모바일은 아직 대응하지 않았습니다.

- [SUNFALL QA 문서](sunfall-qa.md)

## 웹게임 QA

원카드의 규칙·UI 개선 기록과 쇼기·바둑·서사 게임의 자동화 검증 자료를 정리했습니다.

- [웹게임 QA 사례와 테스트 자료](web-games-qa.md)
- [원카드 QA 원본](onecard-qa.docx)

## 메이플스토리 월드 게임

**[메이플 월드] 글로벌 개발 콘테스트 준비 프로젝트**의 별도 QA 사례입니다.

- [콘테스트 준비 노션 — QA 40개와 분석](https://app.notion.com/p/ad782c4386db82aaa1a0010f4f44ed59)

개발 중 플레이하며 남긴 플레이 메모 53개를 중복 제거하여 40개 고유 관찰·개선 제안으로 정리했습니다. 현상과 기대 결과, 재검증 절차를 분리하고 능력치 일관성·강화 반영·사용권 소비를 우선 검증 사례로 선정했습니다.

- [상세 QA 보고서 및 40개 이슈](maple-worlds-qa.md)
- [포트폴리오 사례 페이지](https://onecard-game.onrender.com/qa/maple-worlds/index.html)
- [CSV 목록](../web-games/public/qa/maple-worlds/issues.csv)
- [JSON 데이터](../web-games/public/qa/maple-worlds/issues.json)

상태는 전 항목 ‘재검증 필요’입니다. 원문 취소선만으로 수정 완료를 추정하지 않았으며, 제안한 재검증 절차는 실제 재현·수정·통과 실적과 구분합니다. 게임명과 빌드, 테스트 환경, 영상·로그는 추후 확인 후 보강할 수 있습니다.

## SUNFALL 추가 기록 · 2026-11-06

- [본인 관찰·제보·개선 요청 누적 기록](sunfall-user-qa-history.md): 30개 묶음, 부분 확인과 재검증 대기 구분
- [최근 제보 7건의 상세 리포트](sunfall-regression-2026-11-06.md): 원문 관찰, 코드 수정 대상, 스크린샷 6장, 미실행 재검증 체크리스트 10개
- [본인 기획·제작 활동](../sunfall/07-work-and-decisions.md): 본인 역할과 AI 도구(Codex) 구현 지원 구분
