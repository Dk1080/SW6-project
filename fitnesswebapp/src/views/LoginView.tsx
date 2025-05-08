import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

/*
View that shows the login page and sends a request to login
*/
function LoginView() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const navigate = useNavigate();


    const navigateToDashboard = () => {
        navigate('/dashboard', { replace: true });
    }


    const login = async () => {
        try {
            // Send a login request to the server.
            const body = {
                "username": username,
                "password":password
            }

            const response = await fetch("/api/login", {
                           method: "POST",
                           headers: {
                               "Content-Type": "application/json"
                           },
                           body: JSON.stringify(body)
            });



            //Check if the login details were valid.
            if (response.status == 200) {
                navigateToDashboard();
            } else {
                alert("Invalid details")
            }

            console.log(response)

            } catch (e) {
                console.log(e);
            }
        
    };



  return (
      <div>
          <h1>Please input your login info</h1>
       

          <input type="text" onChange={e => setUsername(e.target.value)} placeholder="Username"></input><br />
          <input type="text" onChange={e => setPassword(e.target.value)} placeholder="Password"></input><br />
          <button onClick={login}>Submit</button>

      </div>
  );
}

export default LoginView;