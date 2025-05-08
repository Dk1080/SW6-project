import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { Doughnut, Pie,  } from 'react-chartjs-2';
ChartJS.register(ArcElement, Tooltip, Legend);
import { useEffect, useState } from "react";
import { useNavigate } from 'react-router-dom';

function DashBoardView() {

    const navigate = useNavigate();
    let requestFlag = false;
    const [chartData, setChartdata] = useState();
    const [userGoalAndPreference, setGoalPreference] = useState();
    const [displayChart, setDisplayChart] = useState({
        labels: [],
        datasets: [
            {
                label: '',
                data: [],
                backgroundColor: [],
                borderColor: [],
                borderWidth: 0,
            },
        ],
    });
    const [options, setOptions] = useState();

    //On site load send a request for graph and goal data.
    useEffect(() => {

        //Function to get health data for graph.
        const getChartData = async () => {
            await fetch("/api/getChartData")
                .then(response => {
                    if (response.status == 200) {

                        console.log(response)

                        response.text().then((resolvedString: string) => {
                            const regularString: string = resolvedString;

                            //console.log(JSON.parse(regularString));
                            setChartdata(JSON.parse(regularString));
                        })

                    }
                })
        };

        //Function to get the goal data and the type of graph to be displayed.
        const getPreferenceData = async () => {
            await fetch("/api/getUserPreferences")
                .then(response => {
                    if (response.status == 200) {

                        console.log(response)

                        response.text().then((resolvedString: string) => {
                            const regularString: string = resolvedString;

                            //console.log(JSON.parse(regularString));
                            setGoalPreference(JSON.parse(regularString))
                        })

                    }
                })
        };

        if (!requestFlag) {
            getChartData();
            getPreferenceData();
            requestFlag = true;
        }


    }, []);

    
    useEffect(() => {
        //Wait for the data to be gotten
        if (chartData && userGoalAndPreference != null) {
            console.log(chartData)
            console.log(userGoalAndPreference)

            //Change what type of graph to display
            switch (userGoalAndPreference.chartPreference) {
                case "Halfcircle":
                    setDisplayChart({
                        labels: [userGoalAndPreference.goals[0].goalType, "Goal"], // Type of goal
                        datasets: [
                            {
                                label: `Number of ${userGoalAndPreference.goals[0].goalType}`,
                                data: [
                                    chartData[0].value,
                                    userGoalAndPreference.goals[0].value - chartData[0].value,
                                ],
                                backgroundColor: [
                                    'rgb(0, 154, 104)',  // Filled portion
                                    'rgb(221, 57, 50)',  // Remaining portion
                                ],
                                borderColor: ['rgba(255, 255, 255, 1)'],
                                borderWidth: 2.5,
                            },
                        ],
                    });

                    setOptions({
                        rotation: -90,
                        circumference: 180,
                        cutout: "60%",
                        maintainAspectRatio: true,
                        responsive: true,
                    });


                    break;
                default:
                    console.log("oh no");
                    break;
            }


            


        }
    }, [chartData, userGoalAndPreference]);



    const goToChatView = () => {
        navigate('/chat', { replace: true });
    }

   




    if (chartData && userGoalAndPreference == null) {
        return (<div>
            <p>Loading...</p>
        </div>)
    } else {
        return (
            <div>
                <div>
                    <Doughnut data={displayChart} options={options} />
                    <p>Steps: {JSON.stringify(chartData?.[0]?.value)}</p>
                    <p>Goal: {JSON.stringify(userGoalAndPreference?.goals?.[0]?.value)}</p>
                </div>
                <button onClick={goToChatView}>Go to Chat</button>
            </div>
        );
    }
    
  
}

export default DashBoardView;