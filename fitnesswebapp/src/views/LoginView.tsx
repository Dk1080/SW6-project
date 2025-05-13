import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

/*
View that shows the login page and sends a request to login
*/
function LoginView() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');


    const [newUsername, setNewUsername] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [newUserResponse, setNewUserResponse] = useState('');

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


    const create = async () => {

        //check if the input is empty
        if (newUsername.length == 0 && newPassword.length == 0) {
            alert("Please fill form.")
            return;
        }

        //Send a request to the server.
        try {
            // Send a login request to the server.
            const body = {
                "username": newUsername,
                "password": newPassword
            }
            setNewUserResponse("Thinking...")
            const response = await fetch("/api/addUser", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(body)
            });

            //Check if the login details were valid.
            if (response.status == 200) {
                setNewUserResponse("User has been created!")
            } else if (response.status == 400) {
                setNewUserResponse("Username is taken");
            }
            else {
                setNewUserResponse("ERROR")
            }

            console.log(response)

        } catch (e) {
            console.log(e);
        }


    }


    const openCloseNewUserForm = () => {
        const formElement = document.getElementById("CreateUserForm");
            formElement.style.display = formElement.style.display === "none" ? "block" : "none";
    }


  return (
      <div>
          <h1>Please input your login info</h1>

          <div>
              <input type="text" onChange={e => setUsername(e.target.value)} placeholder="Username"></input><br />
              <input type="text" onChange={e => setPassword(e.target.value)} placeholder="Password"></input><br />
              <button onClick={login}>Submit</button>
          </div>
          <button onClick={openCloseNewUserForm}>New? create user</button>

          <div id="CreateUserForm">
              <input type="text" onChange={e => setNewUsername(e.target.value)} placeholder="Username"></input><br />
              <input type="text" onChange={e => setNewPassword(e.target.value)} placeholder="Password"></input><br />
              <button onClick={create}>Submit</button>
              <p>{newUserResponse}</p>
          </div>
          
      </div>
  );
}

export default LoginView;