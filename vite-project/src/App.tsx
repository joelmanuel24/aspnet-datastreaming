import React from 'react'
import { useState } from 'react'

function App() {
  const inputRef = React.useRef<HTMLInputElement>(null)
  const [text, setText] = useState('')
  
  const test = async () => {
    setText('')
    const url = 'https://localhost:7236/weatherforecast';
    const data = {
      input: inputRef.current?.value
    }
    fetch(url, { 
      method: 'POST', 
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data) })
    // Retrieve its body as ReadableStream
    .then((response) => {
      const reader = response.body!.getReader();
      console.log(reader)
      return new ReadableStream({
        start(controller) {
          return pump();
          function pump() {
            return reader.read().then(({ done, value }) => {
              if (done) {
                controller.close();
                return;
              }
              setText(text => text + String.fromCharCode(value[0]))
              return pump();
            });
          }
        },
      });
    })
  }
  return (
    <div className="App">
      <div>

      <input type="text" ref={inputRef}/>
      </div>
      <button onClick={test}>test</button>
      <div>
      <div>Output:</div>
      {text}
      </div>
    </div>
  )
}

export default App
