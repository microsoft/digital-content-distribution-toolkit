import React, { Component } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Grid from '@material-ui/core/Grid';
import Switch from '@material-ui/core/Switch';

import { FormControl } from '@material-ui/core';
import TextField from '@material-ui/core/TextField';
import FormGroup from '@material-ui/core/FormGroup';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import { withStyles } from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import Button from '@material-ui/core/Button';
import Typography from '@material-ui/core/Typography';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';
import SaveIcon from '@material-ui/icons/Save';
import DeleteIcon from '@material-ui/icons/Delete';

import ArrowBackIcon from '@material-ui/icons/ArrowBack';
import { Link } from 'react-router-dom';
import { ContentProviderList } from './ContentProviderList';
import { Route } from 'react-router';
import CPDetailForm from './CPDetailForm';



let details='';





export class ContentProviderDetails extends Component {
  static displayName = ContentProviderDetails.name;

  
  
  constructor(props){
    super(props);
    this.state = {contentproviderDetails: 
                    {
                      name:'',
                      contact:'',
                      isActive:false,
                      address: {
                        streetName: '',
                        town: '',
                        city: '',
                        state: '',
                        pin: '',
                        mapLocation: {
                          longitude: 0,
                          latitude: 0
                        }
                      },
                      contentAdministrators: [
                        {
                          contentProviderId: '',
                          isActive: true,
                          activationDate: '',
                          deactivationDate: '',
                          id: '',
                          firstName: '',
                          lastName: '',
                          middleName: '',
                          address: {
                            streetName: '',
                            town: '',
                            city: '',
                            state: '',
                            pin: '',
                            mapLocation: {
                              longitude: 0,
                              latitude: 0
                            }
                          }
                        }
                       ]

                  } , loading:true};
    
  }

  async componentWillMount() {
   await this.populateData(this.props.location.state.details);
  }    
 

  render(props) {

    return (
      <div>
          <CPDetailForm details={this.state.contentproviderDetails}></CPDetailForm>
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
