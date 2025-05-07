
function ChatMessenge(props) {
    //Set the color and posistion of the messasge based on the role.
    let color:string,pos:string;
    if (props.item.role == "assistant") {
        color = "#d3d3d3";
    } else {
        color = "#add8e6";
    }

    return (
            <p className="messasge" style={{ backgroundColor: color, color: "black" }}>
                {props.item.text}
            </p>
    )

}

export default ChatMessenge;