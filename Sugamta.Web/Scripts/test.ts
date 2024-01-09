class Test {
    async register() {
        try {
            const response = await fetch('/Home/Register', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            if (response.ok) {
                // Redirect to the returned view
                window.location.href = response.url;
            } else {
                console.error('Failed to call the server');
            }
        } catch (error) {
            console.error('An error occurred:', error);
        }
    }

    async login() {
        try {
            const response = await fetch('/Home/Index', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            if (response.ok) {
                // Redirect to the returned view
                window.location.href = response.url;
            } else {
                console.error('Failed to call the server');
            }
        } catch (error) {
            console.error('An error occurred:', error);
        }
    }
    /*  async InsertAgent() {
          try {
              const response = await fetch('/Agent/InsertAgent', {
                  method: 'POST',
                  headers: {
                      'Content-Type': 'application/json',
                  }
              });
  
              if (response.ok) {
                  // Redirect to the returned view
                  window.location.href = response.url;
              } else {
                  console.error('Failed to call the server');
              }
          } catch (error) {
              console.error('An error occurred:', error);
          }
      }*/
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