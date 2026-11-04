# GAME LAB 웹게임

[저장소 홈](../README.md) · [프로젝트 상세](PROJECTS.md) · [QA 사례](../qa/README.md)

쇼기·바둑·원카드·서사 미스터리·서바이벌·생태계교란종 RPG 6종을 담은 웹게임 서버입니다. 기존 `test-gamesuber` 저장소에서 이 폴더로 옮겼습니다.

## 실행

Node.js **22.5.0 이상**에서 이 폴더 기준으로 실행합니다.

```bash
npm ci
npm start   # http://localhost:3000 (PORT 환경 변수로 변경)
npm test    # 규칙·UI·멀티플레이 자동화 테스트
```

Windows에서는 `서버시작.bat`을 실행해도 됩니다. AI 도우미 채팅은 `ANTHROPIC_API_KEY` 환경 변수가 필요합니다.

## 구성

| 경로 | 내용 |
| --- | --- |
| [public/](public/) | 게임별 화면·클라이언트·규칙 구현 |
| [server.js](server.js) | 웹 서버와 실시간 연결 (Socket.IO) |
| [gameEngine.js](gameEngine.js) | 원카드 규칙 |
| [shogiMultiplayer.js](shogiMultiplayer.js) · [badukMultiplayer.js](badukMultiplayer.js) | 온라인 대국 처리 |
| [tests/](tests/) | 자동화 테스트 |

## 배포

저장소 루트의 `render.yaml`이 `rootDir: web-games`로 이 폴더를 배포합니다. 기존 Render 서비스가 `test-gamesuber` 저장소에 연결되어 있다면, Render 대시보드에서 연결 저장소를 `game-project-docs`로, Root Directory를 `web-games`로 바꿔야 합니다.

기술: JavaScript · HTML/CSS · Canvas · Node.js · Socket.IO
