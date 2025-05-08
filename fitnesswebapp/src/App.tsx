import {
    BrowserRouter as Router,
    Routes,
    Route,
} from "react-router-dom"; import './App.css'
import LoginView from "./views/LoginView";
import DashBoardView from "./views/DashBoardView";
import ChatView from "./views/ChatView";

function App() {

    return (
        <Router>
            <Routes>
                <Route path="/" element={<LoginView />} />
                <Route path="/dashboard" element={<DashBoardView />} />
                <Route path="/chat" element={<ChatView />} />
            </Routes>
        </Router>
  )
}

export default App
