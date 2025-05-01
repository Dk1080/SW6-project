import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { Pie,  } from 'react-chartjs-2';
ChartJS.register(ArcElement, Tooltip, Legend);
import { useEffect } from "react";
import { useNavigate } from 'react-router-dom';

function DashBoardView() {

    const navigate = useNavigate();

    //On site load send a request for graph data.
    useEffect(() => {
        const getChartData = async () => {
            await fetch("/api/getChartData")
                .then(response => {
                    if (response.status == 200) {

                        console.log(response)

                        response.text().then((resolvedString: string) => {
                            const regularString: string = resolvedString;

                            console.log(JSON.parse(regularString));
                            //const AIresponse = JSON.parse(regularString);
                        })

                    }
                })
        };

        getChartData();
    }, []);

    const goToChatView = () => {
        navigate('/chat', { replace: true });
    }

   


    const data = {
        labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
        datasets: [
            {
                label: '# of Votes',
                data: [12, 19, 3, 5, 2, 3],
                backgroundColor: [
                    'rgba(255, 99, 132, 0.2)',
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(255, 206, 86, 0.2)',
                    'rgba(75, 192, 192, 0.2)',
                    'rgba(153, 102, 255, 0.2)',
                    'rgba(255, 159, 64, 0.2)',
                ],
                borderColor: [
                    'rgba(255, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)',
                ],
                borderWidth: 1,
            },
        ],
    };



    
  return (
      <div>
          <Pie data={data} />;
          <button onClick={goToChatView}>Go to Chat</button>
      </div>
  );
}

export default DashBoardView;