

import { Chart as ChartJS, ArcElement, Tooltip, Legend, CategoryScale, LinearScale, BarElement, plugins } from 'chart.js';
import { Bar, Doughnut, Pie } from 'react-chartjs-2';
import { useEffect, useRef, useState } from "react";
import { useNavigate } from 'react-router-dom';
import ChartjsAnnotation from 'chartjs-plugin-annotation';
import ChartDataLabels from 'chartjs-plugin-datalabels';

ChartJS.register(ArcElement, Tooltip, Legend, CategoryScale, LinearScale, BarElement, ChartjsAnnotation, ChartDataLabels);



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
    const [topOptions, setTopOptions] = useState();
    const [bottomOptions, setBottomOptions] = useState();
    const [currentDate, setCurrentDate] = useState(new Date());
    const [bottomChartData, setBottomChartData] = useState({
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

    const todaysData = useRef(0);
    const goalName = useRef("");
    const [goalsAvalible, setGoalsAvalible] = useState(false);

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

            //Change to better name.
            goalName.current = getRealgoalName(userGoalAndPreference.goals[0]?.goalType);

            //Get the data from today.
            const unixTime = Date.parse(chartData?.[chartData?.length - 1]?.date)
            const tmpDate = new Date(Number(unixTime));
            if (`${tmpDate.getFullYear()}-${tmpDate.getMonth() + 1}-${tmpDate.getDate()}` ===
                `${currentDate.getFullYear()}-${currentDate.getMonth() + 1}-${currentDate.getDate()}`) {
                console.log(chartData[chartData?.length - 1].value)

                todaysData.current = chartData[chartData.length - 1]?.value;
                console.log(todaysData.current)
            }
            console.log(todaysData.current)


            if (userGoalAndPreference.goals.length != 0) {
                setGoalsAvalible(true);
                //Change what type of graph to display
                switch (userGoalAndPreference.chartPreference) {
                    case "Circle":

                        setDisplayChart({
                            labels: [`Number of ${goalName.current} today`, "Remainder to get to goal"], // Type of goal
                            datasets: [
                                {
                                    label: `${goalName.current}`,
                                    data: [
                                        todaysData.current,
                                        Math.max(0, userGoalAndPreference.goals[0].value - todaysData.current),
                                    ],

                                    backgroundColor: [
                                        'rgb(0, 154, 104)',  // Filled portion
                                        'rgb(221, 57, 50)',  // Remaining portion
                                    ],
                                    borderColor: ['rgb(0, 154, 104)', 'rgb(221, 57, 50)'],
                                    borderWidth: 2.5,
                                },
                            ],
                        });

                        setTopOptions({
                            plugins: {
                                legend: {
                                    labels: {
                                        color: 'black',
                                        font: {
                                            size: 20
                                        }
                                    }
                                },
                            },
                            cutout: "60%",
                            maintainAspectRatio: false,
                            responsive: true,
                        });

                        break;
                    case "Column": {
                        console.log(todaysData.current)
                        setDisplayChart({
                            labels: [`Daily ${goalName.current}`],
                            datasets: [{
                                label: `Progress towards goal of ${userGoalAndPreference.goals[0].value} ${goalName.current}`,
                                data: [
                                    [todaysData.current]
                                ],
                                backgroundColor: [
                                    'rgb(0, 154, 104)',
                                ],
                                borderColor: ['rgb(0,0,0)'],
                                barThickness: 'flex',

                                borderWidth: 1
                            }],
                        });
                    }
                        setTopOptions({
                            responsive: true,
                            maintainAspectRatio: false,
                            plugins: {
                                legend: {
                                    labels: {
                                        color: 'black',
                                        font: {
                                            size: 20
                                        }
                                    }
                                },
                                annotation: {
                                    annotations: {
                                        goalLine: {
                                            type: 'line',
                                            yMin: userGoalAndPreference.goals[0].value,
                                            yMax: userGoalAndPreference.goals[0].value,
                                            borderColor: 'red',
                                            borderWidth: 10,
                                        }
                                    }
                                }
                            },
                            layout: {
                                padding: {
                                    top: 20,
                                    right: 20,
                                    bottom: 20,
                                    left: 20
                                }
                            },
                            scales: {
                                x: {
                                    categoryPercentage: 1.0,
                                    barPercentage: 1.0,
                                    ticks: {
                                        color: 'black',
                                        font: {
                                            size: 30
                                        }
                                    }
                                },
                                y: {
                                    ticks: {
                                        color: 'black',
                                        font: {
                                            size: 30
                                        }
                                    }
                                }
                            }

                        });
                        break;
                    default:
                        console.log("oh no");
                        break;


                }
            }

            //Set data for bottom graph.



            //Set variables for use.
            setCurrentDate(new Date());
            console.log(currentDate)
            let labels: string[] = [];
            let data: number[] = [];
            let parsedDate: Object[] = [];

            if (!userGoalAndPreference?.goals?.[0]?.interval) {
                return;
            }

            //Switch based on what interval it is.
            switch (userGoalAndPreference?.goals?.[0].interval) {
                
                case "weekly":
                    {

                        //Parse the seven latest days from the database into date format.
                        for (let i = 0; i < 7; i++) {
                            let unixTime = Date.parse(chartData?.[i]?.date).toString();
                            let tmpDate = new Date(Number(unixTime));

                            if (!isNaN(tmpDate)) {
                                parsedDate.push({ "date": `${tmpDate.getFullYear()}-${tmpDate.getMonth() + 1}-${tmpDate.getDate()}`, "value": chartData?.[i]?.value });
                            }
                        }

                        console.log(parsedDate)
                        //Go thorugh each the of the dates to check if they are within a week.
                        for (let i = 0; i < 7; i++) {
                            let found = false;

                            //From today shift add the date to the label
                            let shiftedDate = new Date(currentDate);
                            shiftedDate.setDate(currentDate.getDate() - i);
                            labels.unshift(`${shiftedDate.getMonth() + 1}-${shiftedDate.getDate()}`);

                            //Check if the date has a step value.

                            parsedDate.forEach((pDate: any) => {
                                if (pDate.date === `${shiftedDate.getFullYear()}-${shiftedDate.getMonth() + 1}-${shiftedDate.getDate()}`) {
                                    data.unshift(pDate.value);
                                    found = true;
                                }
                            });

                            if (!found) {
                                data.unshift(0)
                            }


                        }
                        console.log(labels)
                        console.log(data)

                        //Set the data and label to the bottom graph.
                        setBottomChartData({
                            labels: labels,
                            datasets: [
                                {
                                    label: `${goalName.current}`,
                                    data: data,
                                    backgroundColor: "rgb(0, 139, 95)"
                                }
                            ]
                        });
                        setBottomOptions({
                            plugins: {
                                datalabels: {
                                    color: "white",
                                    font: {
                                        size: 15,
                                    }
                                }
                            },
                            scales: {
                                x: {
                                    categoryPercentage: 1.0,
                                    barPercentage: 1.0,
                                    ticks: {
                                        color: 'white',
                                        font: {
                                            size: 20
                                        }
                                    }
                                },
                                y: {
                                    ticks: {
                                        color: 'white',
                                        font: {
                                            size: 20
                                        }
                                    }
                                }
                            }

                        })

                    }
                    break;
                case "biweekly":
                    {
                        //Parse the 14 latest days from the database into date format.
                        for (let i = 0; i < 14; i++) {
                            let unixTime = Date.parse(chartData?.[i]?.date).toString();
                            let tmpDate = new Date(Number(unixTime));

                            if (!isNaN(tmpDate)) {
                                parsedDate.push({ "date": `${tmpDate.getFullYear()}-${tmpDate.getMonth() + 1}-${tmpDate.getDate()}`, "value": chartData?.[i]?.value });
                            }
                        }

                        console.log(parsedDate)
                        //Go thorugh each the of the dates to check if they are within two week.
                        for (let i = 0; i < 14; i++) {
                            let found = false;

                            //From today shift add the date to the label
                            let shiftedDate = new Date(currentDate);
                            shiftedDate.setDate(currentDate.getDate() - i);
                            labels.unshift(`${shiftedDate.getFullYear()}-${shiftedDate.getMonth() + 1}-${shiftedDate.getDate()}`);

                            //Check if the date has a step value.

                            parsedDate.forEach((pDate: any) => {
                                if (pDate.date === `${shiftedDate.getFullYear()}-${shiftedDate.getMonth() + 1}-${shiftedDate.getDate()}`) {
                                    data.unshift(pDate.value);
                                    found = true;
                                }
                            });

                            if (!found) {
                                data.unshift(0)
                            }


                        }
                        console.log(labels)
                        console.log(data)

                        //Set the data and label to the bottom graph.
                        setBottomChartData({
                            labels: labels,
                            datasets: [
                                {
                                    label: `${goalName.current}`,
                                    data: data,
                                    backgroundColor: "rgb(0, 139, 95)"
                                }
                            ]
                        });
                        setBottomOptions({
                            scales: {
                                x: {
                                    categoryPercentage: 1.0,
                                    barPercentage: 1.0,
                                    ticks: {
                                        color: 'white',
                                        font: {
                                            size: 20
                                        }
                                    }
                                },
                                y: {
                                    ticks: {
                                        color: 'white',
                                        font: {
                                            size: 20
                                        }
                                    }
                                }
                            }

                        })


                    }
                    break;
                case "monthly":
                    //Parse the 30 latest days from the database into date format.
                    for (let i = 0; i < 30; i++) {
                        let unixTime = Date.parse(chartData?.[i]?.date).toString();
                        let tmpDate = new Date(Number(unixTime));

                        if (!isNaN(tmpDate)) {
                            parsedDate.push({ "date": `${tmpDate.getFullYear()}-${tmpDate.getMonth() + 1}-${tmpDate.getDate()}`, "value": chartData?.[i]?.value });
                        }
                    }

                    console.log(parsedDate)
                    //Go thorugh each the of the dates to check if they are within a month.
                    for (let i = 0; i < 30; i++) {
                        let found = false;

                        //From today shift add the date to the label
                        let shiftedDate = new Date(currentDate);
                        shiftedDate.setDate(currentDate.getDate() - i);
                        labels.unshift(`${shiftedDate.getFullYear()}-${shiftedDate.getMonth() + 1}-${shiftedDate.getDate()}`);

                        //Check if the date has a step value.

                        parsedDate.forEach((pDate: any) => {
                            if (pDate.date === `${shiftedDate.getFullYear()}-${shiftedDate.getMonth() + 1}-${shiftedDate.getDate()}`) {
                                data.unshift(pDate.value);
                                found = true;
                            }
                        });

                        if (!found) {
                            data.unshift(0)
                        }


                    }
                    console.log(labels)
                    console.log(data)

                    //Set the data and label to the bottom graph.
                    setBottomChartData({
                        labels: labels,
                        datasets: [
                            {
                                label: `${goalName.current}`,
                                data: data,
                                backgroundColor: "rgb(0, 139, 95)"
                            }
                        ]
                    });
                    setBottomOptions({
                        plugins: {
                            datalabels: {
                                display: true,
                                color: 'white',
                                font: {
                                    size: 15,
                                },
                                anchor: 'end',
                                align: 'top',
                                formatter: (value: number) => value, // Show the value
                            }
                        },
                        scales: {
                            x: {
                                categoryPercentage: 1.0,
                                barPercentage: 1.0,
                                ticks: {
                                    color: 'black',
                                    font: {
                                        size: 15
                                    }
                                }
                            },
                            y: {
                                ticks: {
                                    color: 'black',
                                    font: {
                                        size: 15
                                    }
                                }
                            }
                        }

                    })


                    break;
                default:
                    console.log("Oh no^2")
                    break;

            }          
        }
    }, [chartData, userGoalAndPreference]);



    const goToChatView = () => {
        navigate('/chat', { replace: true });
    }

   


    const getRealgoalName = (goalName: string): string => {

        switch (goalName) {
            case "ActiveCaloriesBurnedRecord":
                return "active calories burned";
            case "TotalCaloriesBurnedRecord":
                return "total calories burned";
            case "DistanceRecord":
                return "distance";
            case "ElevationGainedRecord":
                return "elevation gained";
            case "FloorsClimbedRecord":
                return "floors climbed";
            case "HeartRateRecord":
                return "heart rate";
            case "HeightRecord":
                return "height";
            case "RestingHeartRateRecord":
                return "resting heart rate";
            case "StepsRecord":
                return "steps";
            case "WeightRecord":
                return "weight";
            case "WheelchairPushesRecord":
                return "wheelchair pushes";
            default:
                return "ERROR";
        }



    }




    if (chartData && userGoalAndPreference == null) {
        return (<div>
            <p>Loading...</p>
        </div>)
    } else {
        return (
            <div>
                <h1 style={{marginBottom:-70}}>Dashboard</h1>

                {goalsAvalible == true ? (
                    <div style={{ backgroundColor: "rgb(0, 115, 113)", borderRadius: '25px', padding: "5px", margin: "100px", width: Math.floor(window.innerWidth * 0.7), height: Math.floor(window.innerHeight * 0.4) }}>
                        {userGoalAndPreference?.chartPreference === "Circle" && <Doughnut data={displayChart} options={topOptions} />}
                        {userGoalAndPreference?.chartPreference === "Column" && <Bar data={displayChart} options={topOptions} />}
                        <h2 style={{ display: 'inline-block', padding: '0px 10px' }}> Today's {goalName.current}: {todaysData.current} </h2>
                        <h2 style={{ display: 'inline-block' }}> Goal: {JSON.stringify(userGoalAndPreference?.goals?.[0]?.value)}</h2>
                    </div>
                ) : (
                        <div style={{display:"flex", backgroundColor: "rgb(0, 115, 113)", borderRadius: '25px', padding: "5px", margin: "100px", width: Math.floor(window.innerWidth * 0.6), height: Math.floor(window.innerHeight * 0.2) }}>
                            <p style={{textAlign:'center', verticalAlign:"middle"}}>No goals available. Please set a goal to see your dashboard information.</p>
                    </div>
                )}


                <div style={{ backgroundColor: "rgb(139, 1, 1)", borderRadius: '25px', margin: "100px" }}>
                    <Bar data={bottomChartData} options={bottomOptions} />
                </div>


                <button style={{ marginTop:-50, width: Math.floor(window.innerWidth * 0.7), backgroundColor: "rgb(62, 97, 208)" }} onClick={goToChatView}>
                    <img src={`/chat.svg`} alt="Button Icon" style={{ height: Math.floor(window.innerHeight * 0.09) }} />
                </button>
            </div>
        );
    }
    
  
}

export default DashBoardView;
