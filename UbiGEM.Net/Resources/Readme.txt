### 2.0.2.3 : 2022/10/14
  -. Variable에 custom child item 추가 기능 : 복잡한 구조를 가진 경우 Add method를 이용하여 동일 level에서 child 추가하는 기능

### 2.0.2.2 : 2022/09/21
  -. Bug Fix : UbiGEM Init 실패 후 Start 시 App crash 발생 해결

### 2.0.2.1 : 2022/07/22
  -. Keylock log 추가 : Keylock-OnRequestLogging

### 2.0.2.0 : 2022/06/13
  -. .net frame work 4.5 적용
  -. 코드 정리

#### 2.0.1.6 : 2021/06/02
  -. S2F25/S2F26 : Length=1인 ABS 송/수신 대응

#### 2.0.1.5 : 2021/05/04
  -. Control State가 Equipment Offline 상태에서 Request Online(S1F17) 허용 여부 Option 처리 추가 함

#### 2.0.1.4 : 2021/04/26
  -. ReportCollectionEvent List<VariableInfo> variables 추가 : variables에서 우선 검색 후 없을 경우 Driver가 가진 전체에서 variable을 찾아 S6F11 생성 함.
  -. ReportCollectionEvent List<string> vids, List<string> values 추가 : vids에서 우선 검색 후 없을 경우 Driver가 가진 전체에서 variable을 찾아 S6F11 생성 함.
                                                                         (Driver가 가진 전체에서 variable에 value update 하지 않음)

#### 2.0.1.3 : 2021/04/19
  -. S1F18 ACK 시 S1F1 없이 online 전환하도록 수정 함
  
#### 2.0.0.3 : 2021/12/16
  -. Disconnected 상태에서 message send 시 log 보완