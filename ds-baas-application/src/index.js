import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import './Form.css';
import reportWebVitals from './reportWebVitals';
import Amplify, { Auth } from "aws-amplify";
import awsExports from "./aws-exports";
Amplify.configure({
  Auth: {

    // REQUIRED only for Federated Authentication - Amazon Cognito Identity Pool ID
    identityPoolId: 'eu-west-1:ba6f3f94-e5fc-4362-9c88-2e3c9f4b4511',

    // REQUIRED - Amazon Cognito Region
    region: 'eu-west-1',

    // OPTIONAL - Amazon Cognito Federated Identity Pool Region 
    // Required only if it's different from Amazon Cognito Region
    identityPoolRegion: 'eu-west-1',

    // OPTIONAL - Amazon Cognito User Pool ID
    userPoolId: 'eu-west-1_ciAlya1Fo',

    // OPTIONAL - Amazon Cognito Web Client ID (26-char alphanumeric string)
    userPoolWebClientId: '2qjo5qv80rb4htada9bkhtrnsu',

    // OPTIONAL - Enforce user authentication prior to accessing AWS resources or not
    //mandatorySignIn: false,

    // OPTIONAL - Configuration for cookie storage
    // Note: if the secure flag is set to true, then the cookie transmission requires a secure protocol
    //cookieStorage: {
    // REQUIRED - Cookie domain (only required if cookieStorage is provided)
    //    domain: '.yourdomain.com',
    // OPTIONAL - Cookie path
    //    path: '/',
    // OPTIONAL - Cookie expiration in days
    //    expires: 365,
    // OPTIONAL - See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie/SameSite
    //    sameSite: "strict" | "lax",
    // OPTIONAL - Cookie secure flag
    // Either true or false, indicating if the cookie transmission requires a secure protocol (https).
    //    secure: true
    //},

    // OPTIONAL - customized storage object
    //storage: MyStorage,

    // OPTIONAL - Manually set the authentication flow type. Default is 'USER_SRP_AUTH'
    //authenticationFlowType: 'USER_PASSWORD_AUTH',

    // OPTIONAL - Manually set key value pairs that can be passed to Cognito Lambda Triggers
    //clientMetadata: { myCustomKey: 'myCustomValue' },
    //clientMetadata: {},
    // OPTIONAL - Hosted UI configuration
    oauth: {
      //    domain: 'your_cognito_domain',
      //    scope: ['phone', 'email', 'profile', 'openid', 'aws.cognito.signin.user.admin'],
      //    redirectSignIn: 'http://localhost:3000/',
      //    redirectSignOut: 'http://localhost:3000/',
      //    responseType: 'code' // or 'token', note that REFRESH token will only be generated when the responseType is code
    }
  }
});

const currentConfig = Auth.configure();

class Form extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      email: '',
      password: '',
      token: '',
      loggedUser: {
        id: "", name: "", surname: "", email: ""
      },
      logged: false,
      loginfail:false

    }

    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }
  async callCognitoLogin(email, password) {
    try {
      console.log(email, password);
      const user = await Auth.signIn(email, password);
      
      await Auth.currentSession().then(res => {
        let accessToken = res.getIdToken()
        let jwt = accessToken.getJwtToken();
        this.setState({ token: jwt })
        console.log(`Bearer ${jwt}`)
        console.log(this.state.token);
      });
      fetch('https://7ge1zdatk1.execute-api.eu-west-1.amazonaws.com/Prod/api/Users/' + user.username, {
        method: 'GET',
        headers: new Headers({
          'Authorization': 'Bearer ' + this.state.token,
          'Content-Type': 'application/x-www-form-urlencoded'
        })
      }).then(res => res.json()
      ).then(response => {
        this.setState({ loggedUser: response });
        this.setState({ logged: true })
        console.log(response);
      });
    
    } catch (error) {
      alert('login fail, uncorrect username or password')
      console.log('error signing in', error)
    }


  }
  handleChange(event) {
    let nam = event.target.name;
    let val = event.target.value;
    this.setState({ [nam]: val })
  }
  async handleSubmit(event) {
    event.preventDefault();
    await this.callCognitoLogin(this.state.email, this.state.password);
  }
  render() {
    if (!this.state.logged) {
      return (
        <div>
          <form className="formPanel" onSubmit={this.handleSubmit} >

            <label htmlFor="email"> Email:</label>
            <input id="email" type="text" name="email" value={this.state.email} onChange={this.handleChange} />

            <label htmlFor="password">password:</label>
            <input id="password" type="password" name="password" value={this.state.password} onChange={this.handleChange} />
            <input className="button" type="submit" value="Submit" />

          </form ></div>)
    } else {
      return (
        <div className="responsePanel">
          <h5 className="responseLabel"> id: </h5>
          <p className="responseLabel"> {this.state.loggedUser.id}</p>
          <h5 className="responseLabel"> name: </h5>
          <p className="responseLabel"> {this.state.loggedUser.name} </p>
          <h5 className="responseLabel"> surname: </h5>
          <p className="responseLabel"> {this.state.loggedUser.surname} </p>
          <h5 className="responseLabel"> Email: </h5>
          <p className="responseLabel"> {this.state.loggedUser.email} </p>
        </div>)
    }

  }
}

class Page extends React.Component {

  render() {
    return (
      <div className="page">
        <div className="form">
          <Form />
        </div>
      </div>
    )
  }
}
//const REACT_VERSION = React.version;
ReactDOM.render(
  //<div>React version: {REACT_VERSION}</div>,
  <Page />,
  document.getElementById('root')
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
