import React, { Component } from 'react';
import BineTable from '../BineTable';

function createData(name, contact, address) {
  return { 
    name, 
    contact, 
    address };

}

const cpList = [
  createData('Mishtu Mobile Booth', '+91 9231232341', 'J.P. nagar, Banglore'),
  createData('Apple Care', '+91 9333232341', 'A.M. nagar, Banglore'),
  createData('Dell Center', '+91 9451232341', 'Bilekahalli, Banglore'),
  createData('Random Grocery store', '+91 9454532341', 'Richmond Circle, Banglore'),
  createData('Random Tapri', '+91 9231244441', 'Pimpri, Pune'),
  createData('Small business venture', '+91 9222329341', 'Worli, Mumbai'),
  createData('Medium Business', '+91 5454433441', 'Delhi, India'),
  createData('Blendnet Mobiles', '+91 8234232341', 'Whitefield, Banglore'),
  createData('Samsung Smart booth', '+91 9444442341', 'Bellandur, Banglore'),
  createData('Motorola Store', '+91 8888832341', 'Electronic city, Banglore'),
  createData('Dmart', '+91 9231856233', 'Harlur road, Banglore'),
  createData('Reliance', '+91 9245632341', 'Ranka colony, Banglore'),
  createData('List Exhaused', '+91 8981232341', 'JayaNagar, Banglore'),
];

export class ContentProviderList extends Component {
    static displayName = ContentProviderList.name;
    constructor(props){
        super(props);
        this.state = {contentproviders: [], loading:true};
    }

    componentDidMount() {
       
        this.populateData();
    }    

    static renderContentProvidersTable(contentproviders) {
        return (
          <table className='table table-striped' aria-labelledby="tabelLabel">
            <thead>
              <tr>
                <th>Name</th>
                <th>Contact Number</th>
              </tr>
            </thead>
            <tbody>
              {contentproviders.map(contentprovider =>
                <tr key={contentprovider.id}>
                  <td>{contentprovider.name}</td>
                  <td>{contentprovider?.contentProviderContact?.contactNumber}</td>
                  
                </tr>
              )}
            </tbody>
          </table>
        );
    }

    render() {
        let contents = this.state.loading
          ? <p><em>Loading...</em></p>
          : ContentProviderList.renderContentProvidersTable(this.state.contentproviders);
    
        return (
          <div>
            {/* <h1 id="tabelLabel" >Content Providers</h1>
            <p>BlendNet registered ContentProviders list.</p>
            {contents} */}
            <div>
              <BineTable rows={this.state.contentproviders} />
            </div>
            
          </div>
        );
      }
    async populateData() {
        
        const serviceName='https://localhost:5001/api/v1/ContentProviders';
        const response = await fetch(serviceName);
        const data = await response.json();
        
        
        this.setState({ contentproviders: data, loading: false });
        
      }
      
}
