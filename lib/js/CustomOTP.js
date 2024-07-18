class OTPManager {
    /**
     * OTPManager 생성자
     * @param formSelector form 요소의 셀렉터
     * @param fieldSelector OTP 입력 필드들 (input 요소)의 셀렉터
     * @param options 추가 옵션들. (invalidClass: 입력값이 유효하지 않을 때 추가할 클래스명, onInvalid: 입력값이 유효하지 않을 때 호출할 콜백 함수)
     */
    constructor(formSelector, fieldSelector, options = {}) {
        this.form = document.querySelector(formSelector);
        this.fields = this.form.querySelectorAll(fieldSelector);
        this.options = Object.assign({
            invalidClass: 'is-invalid',
            onInvalid: () => {
            }
        }, options);

        this.initFields();
    }
    /**
     * OTP 입력 필드들에 이벤트 리스너를 등록하는 메서드
     */
    initFields() {
        this.fields.forEach((field, index) => {
            field.addEventListener('input', (e) => this.handleInput(e, index));
            field.addEventListener('keydown', (e) => this.handleKeydown(e, index));
            field.addEventListener('focus', () => this.setInvalid(false));
            field.addEventListener('paste', (e) => this.handlePaste(e, index));
        });
    }

    /**
     * OTP 입력 필드에 입력된 값이 변경될 때 호출되는 메서드
     * @param e input 이벤트 객체
     * @param index 현재 입력 필드의 인덱스 (forEach로 인해 자동으로 증가되는 값)
     */
    handleInput(e, index) {
        if (e.target.value.length === 1 && index < this.fields.length - 1) {
            this.fields[index + 1].focus();
        }
        this.setInvalid(false);
    }

    /**
     * OTP 입력 필드에서 키보드 입력이 발생했을 때 호출되는 메서드
     * @param e keydown 이벤트 객체
     * @param index 현재 입력 필드의 인덱스 (forEach로 인해 자동으로 증가되는 값)
     */
    handleKeydown(e, index) {
        if (e.key === 'Backspace' && index > 0 && e.target.value === '') {
            this.fields[index - 1].focus();
            this.setInvalid(false);
        }
    }

    /**
     * OTP 입력 필드에서 붙여넣기 이벤트가 발생했을 때 호출되는 메서드
     * @param e paste 이벤트 객체
     * @param index 현재 입력 필드의 인덱스 (forEach로 인해 자동으로 증가되는 값)
     */
    handlePaste(e, index) {
        e.preventDefault();
        const pastedData = e.clipboardData.getData('text').slice(0, this.fields.length - index);

        pastedData.split('').forEach((char, i) => {
            if (index + i < this.fields.length) {
                this.fields[index + i].value = char;
            }
        });

        // 붙여넣기 후 마지막 입력 필드로 포커스 이동
        const lastFilledIndex = Math.min(index + pastedData.length, this.fields.length) - 1;
        this.fields[lastFilledIndex].focus();
    }

    /**
     * OTP 입력 필드들의 값을 합쳐서 반환하는 메서드
     * @returns {string} OTP 입력 필드들의 값을 합친 문자열
     */
    getOTPValue() {
        return Array.from(this.fields).map(input => input.value).join('');
    }

    /**
     * OTP 입력 필드들에 유효하지 않은 입력값을 표시하는 메서드. 이때, 옵션으로 전달한 onInvalid 콜백 함수를 호출한다.
     * @param isInvalid bool 데이터 타입. 입력값이 유효하지 않은지 여부
     */
    setInvalid(isInvalid) {
        this.fields.forEach(field => {
            field.classList.toggle(this.options.invalidClass, isInvalid);
        });
        if (isInvalid) {
            this.options.onInvalid();
        }
    }

    /**
     * OTP 입력 필드 중 특정 인덱스에 포커스를 주는 메서드
     * @param index 포커스를 줄 OTP 입력 필드의 인덱스
     */
    focus(index) {
        this.fields[index].focus();
    }

    /**
     * OTP 입력 필드들의 값을 모두 초기화하는 메서드. 초기화 후 첫 번째 입력 필드에 포커스를 준다.
     */
    AllClear() {
        this.fields.forEach(field => field.value = '');
        this.focus(0);
    }
}

/**
 * OTPManager 인스턴스를 생성합니다.
 * @param formSelector OTP 입력 필드들을 감싸는 form 요소의 셀렉터
 * @param fieldSelector OTP 입력 필드들 (input 요소)의 셀렉터
 * @param options 추가 옵션들. (invalidClass: 입력값이 유효하지 않을 때 추가할 클래스명, onInvalid: 입력값이 유효하지 않을 때 호출할 콜백 함수)
 * @returns {OTPManager}
 */
function createOTPManager(formSelector, fieldSelector, options = {}) {
    return new OTPManager(formSelector, fieldSelector, options);
}