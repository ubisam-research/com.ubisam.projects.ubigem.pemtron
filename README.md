# com.ubisam.projects.ubigem.pemtron

Pemtron 버전 UbiGEM 소스 레포지토리입니다.  
**빌드/배포 절차는 basic과 동일**하며, **Pemtron 전용 변경점은 소스 내 `4pemtron` 표시 구간**에 반영되어 있습니다.

---

## 1. 빌드 절차 (basic과 동일)

> 사전 조건: `BIN\ConfuseEx` 폴더가 존재해야 합니다.

1. 탐색기에서 `UbiSam.Sources.Revision.sln` 실행
2. **`UbiSam.KeyLock` 빌드**
3. **`UbiCom.Net.Automata.SECS2` 빌드**
4. **`UbiSam.KeyLock` 재빌드**  
   - `UbiCom.Net.Automata.SECS2` 빌드 후 난독화 과정을 거치므로  
     `UbiGEM.Net` 빌드 전에 **반드시 KeyLock을 한 번 더 빌드**해야 합니다.
5. **`UbiGEM.Net` 빌드**
   - 빌드 완료 후 `BIN\Confused` 폴더가 생성됩니다.
6. `BIN\Confused` 폴더 내 **4개 파일**을 `Setup\BIN Data\UbiGEM\BIN` 폴더로 복사
7. `Setup\UbiGEM` 폴더에서 `UbiGEM.sln` 실행
8. **`UbiGEM` 빌드**
9. `Setup\Setup Files\` 폴더에 생성된 **`UbiGEM.msi`**를 ZIP으로 압축하여 배포 패키지 생성

---

## 2. 배포 방식 (현재 운영 방식)

현재는 위의 **1 ~ 8 빌드 과정을 생략**하고,  
**배포 ZIP을 그대로 고객에게 제공**하는 방식으로 운영 중입니다.

- Repository: `com.ubisam.projects.ubigem.dist`
- 배포 파일: **`UbiGEM_20211221.zip`**

---

## 3. UbiCOM 빌드 & 배포

1. **UbiCOM은 제공된 소스 기준으로 빌드 및 배포 가능**
2. `Setup\UbiCOM` 폴더에서 `UbiCOM.sln` 실행 후 빌드

---

## 4. UbiGEM.PAC

- **UbiGEM.PAC 소스 및 빌드 절차는 구미 쪽 추가 확인 필요**

---

## 5. KeyLock (키락) 생성

- **장석주 부장님을 통해 KeyLock 1.0 생성**
- UbiGEM / UbiGEM.PAC 판매 시, **키락 생성 및 발송 절차는 장석주 부장님 통해 진행**

---

## 6. Pemtron 버전에서 달라지는 부분

- **기본 빌드/배포 절차는 basic과 동일**
- **Pemtron 전용 변경점은 소스 내 `4pemtron` 표기된 코드/구간에만 존재**
  - Pemtron 관련 수정 범위를 확인할 때 `4pemtron` 검색을 권장
