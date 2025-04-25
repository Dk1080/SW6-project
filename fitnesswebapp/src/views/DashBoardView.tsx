import { useEffect } from "react";
import { useNavigate } from 'react-router-dom';

function DashBoardView() {

    const navigate = useNavigate();

    //On site load send a request for graph data.
    useEffect(() => {
        const getChartData = async () => {
            const response = await fetch("/api/getChartData");
            console.log(response)
        };

        getChartData();
    }, []);

    const goToChatView = () => {
        navigate('/chat', { replace: true });
    }
    
  return (
      <div>
          <button onClick={goToChatView}>Go to Chat</button>
      </div>
  );
}

export default DashBoardView;