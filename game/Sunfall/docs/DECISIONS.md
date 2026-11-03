# 기획과 첫 구현의 경계

## 사용자가 정한 것

- PC·모바일 싱글 로그라이크.
- 태양이 사라진 세계에서 원인을 찾고 태양을 되찾는 이야기.
- 기본 방향키 이동, Ctrl 공격, Alt 점프, 사용자 키 변경.
- 참고작은 스컬(Skul). 사용자가 스토리와 기획을 결정한다.

## 첫 프로토타입의 임시 선택

- Unity 6.0, C#, 2D 횡스크롤 실시간 액션. Visual Studio는 코드 편집기로 사용.
- 작업명 Sunfall. 주인공·적·보상 이름, 그래픽·음악·전투 수치는 임시.
- 연속 지면 위 안전하게 연결된 전투방 3개 + 보스 1개. 랜덤 발판과 적 배치.
- 근접 공격, 단일 점프, 자동 타깃 추적 적, 보스 지면 공격.
- 왼쪽·오른쪽 Ctrl/Alt는 같은 키, Esc 예약, 중복 바인딩은 거부.
- 키 설정 변경은 즉시 저장. 재할당 대기만 Esc/취소 버튼으로 취소.
- 영구 능력치 성장 없음. 런 저장 없음. 키/음량만 로컬 설정으로 저장.
- 모바일 가로 화면, 고정 터치 버튼. OS·버튼 편집 지원 범위는 추후 결정.

## 다음에 기획할 항목

태양이 사라진 원인, 주인공의 동기, 첫 단서, 고유 빌드 시스템, 회피/스킬, 보상 조합,
방 분기와 상점, 사망의 서사적 의미, 영구 성장, 이어하기, 정식 미술과 BGM.

스컬의 캐릭터·맵·음원·이미지를 가져오지 않았으며 머리 교체를 그대로 구현하지 않았다.

## 기술 참고

- https://docs.unity3d.com/kr/6000.0/Manual/com.unity.inputsystem.html
- https://docs.unity3d.com/Packages/com.unity.inputsystem@1.8/manual/Touch.html
- https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/runtimeinitializeonloadmethodattribute
