import React, { Component } from 'react';
import CPDetailForm from './CPDetailForm';

let details='';
export class ContentProviderDetail extends Component {
  static displayName = ContentProviderDetail.name;

  constructor(props){
    super(props);
    this.state = {contentproviderDetails: null, loading:true};
    //this.populateData(this.props.location.state.details);
  }

  async componentWillMount() {
   await this.populateData(this.props.location.state.details);
  }    
 

  render(props) {
   
    
      return (
        <div>
            {/* <CPDetailForm details={this.props.location.state.details}></CPDetailForm> */}
            {/* <CPDetailForm details={this.state.contentproviderDetails}></CPDetailForm> */}
        </div>
      );
    
  }

  async populateData(providerDetails) {
    
    const id = providerDetails.id;
    const serviceName='https://localhost:5001/api/v1/ContentProviders/'+id;
    const response = await fetch(serviceName);
    const data = await response.json();
    
    this.setState({ contentproviderDetails: data, loading: false });

    //alert(this.state.contentproviderDetails.name);
  }
}
