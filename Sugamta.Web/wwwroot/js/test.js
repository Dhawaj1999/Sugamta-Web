var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class Test {
    register() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                const response = yield fetch('/Home/Register', {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                    }
                });
                if (response.ok) {
                    // Redirect to the returned view
                    window.location.href = response.url;
                }
                else {
                    console.error('Failed to call the server');
                }
            }
            catch (error) {
                console.error('An error occurred:', error);
            }
        });
    }
    login() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                const response = yield fetch('/Home/Index', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    }
                });
                if (response.ok) {
                    // Redirect to the returned view
                    window.location.href = response.url;
                }
                else {
                    console.error('Failed to call the server');
                }
            }
            catch (error) {
                console.error('An error occurred:', error);
            }
        });
    }
}
function loadRegister() {
    const tester = new Test();
    tester.register();
}
function loadLogin() {
    const tester = new Test();
    tester.login();
}
/*function loadAgent() {
    const tester = new Test();
    tester.InsertAgent();
}*/ 
//# sourceMappingURL=test.js.map