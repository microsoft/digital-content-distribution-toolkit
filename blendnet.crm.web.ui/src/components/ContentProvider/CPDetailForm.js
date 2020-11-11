import React from 'react';
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
import axios from 'axios';

let details = null;

function populateData(contentProviderId){

}
const useStyles = makeStyles((theme) => ({
  root: {
    '& .MuiTextField-root': {
      margin: theme.spacing(1),
      width: '26ch',
    },
  },
  card: {
    minWidth: 200,
  },
  bullet: {
    display: 'inline-block',
    margin: '0 2px',
    transform: 'scale(0.8)',
  },
  title: {
    fontSize: 14,
  },
  pos: {
    marginBottom: 12,
  },
  link: {
    textDecoration: 'none',
    color: 'black'
  },
  button: {
    margin: theme.spacing(1),
    backgroundColor: '#bababa',
    '&:hover': {
      backgroundColor: '#bababa',
  },
  color: '#0078d4',
  },
  buttonDelete : {
    margin: theme.spacing(1),
    backgroundColor: '#bababa',
    '&:hover': {
      backgroundColor: '#bababa',
    },
    color: 'red',
  },
}
));

const StatusSwitch = withStyles((theme) => ({
  root: {
    width: 42,
    height: 26,
    padding: 0,
    margin: theme.spacing(1),
  },
  switchBase: {
    padding: 1,
    '&$checked': {
      transform: 'translateX(16px)',
      color: theme.palette.common.white,
      '& + $track': {
        backgroundColor: '#52d869',
        opacity: 1,
        border: 'none',
      },
    },
    '&$focusVisible $thumb': {
      color: '#52d869',
      border: '6px solid #fff',
    },
  },
  thumb: {
    width: 24,
    height: 24,
  },
  track: {
    borderRadius: 26 / 2,
    border: `1px solid ${theme.palette.grey[400]}`,
    backgroundColor: theme.palette.grey[50],
    opacity: 1,
    transition: theme.transitions.create(['background-color', 'border']),
  },
  checked: {},
  focusVisible: {},
}))(({ classes, ...props }) => {
  return (
    <Switch
      focusVisibleClassName={classes.focusVisible}
      disableRipple
      classes={{
        root: classes.root,
        switchBase: classes.switchBase,
        thumb: classes.thumb,
        track: classes.track,
        checked: classes.checked,
      }}
      {...props}
    />
  );
});



export default function DetailForm(props) {
  
  const classes = useStyles();
  const [state, setState] = React.useState({
    status: true,
  });

const [openSave, setOpenSave] = React.useState(false);
const [openDelete, setOpenDelete] = React.useState(false);
const [details, setDetails] = React.useState(props.details);



const handleCloseSave = () => {
  setOpenSave(false);
};

const handleClickOpenSave = () => {
  setOpenSave(true);

  let contentProvider=null;
  // let contentProvider={
  //   Id:this.refs.Id.value,
  //   Name:this.refs.Name.value,
  //   Location:this.refs.Location.value,
  //   Salary:this.refs.Salary.value

  
  fetch('https://localhost:5001/api/v1/ContentProviders',{
      method: 'POST',
      headers:{'Content-type':'application/json'},
        body: contentProvider
    }).then(r=>r.json()).then(res=>{
      if(res){
        this.setState({message:'Content Provider Updated Successfully'});
      }
    });
};


const handleCloseDelete = () => {
  setOpenDelete(false);
};

const handleClickOpenDelete = () => {
  setOpenDelete(true);
};

  const handleChange = (event) => {
    setState({ ...state, [event.target.name]: event.target.checked });
  };

  const updateValues = (e) =>{
    //alert('ok');
    //alert(event.target.name + " "+ event.target.value);
    setDetails({ [e.target.name]: e.target.value });
    
  };
 
  const onSubmit = (e) => {

    e.preventDefault();
    // get our form data out of state
    console.log(details);
    //const { fname, lname, email } = this.details;

    // axios.post('/', { fname, lname, email })
    //   .then((result) => {
    //     //access the results here....
    //   });
    setOpenSave(false);  
  }

  if(props.details==null){
    return(
      <div></div>
    );
  }
  else
  {
    console.log(props.details);
    return (
      <div>
        <Route path='/content-provide-list' component={ContentProviderList} />
            <div>
            <Grid container spacing={0}>
                <Grid item xs={9}>
                <h1>
                    Content Provider Detail
                </h1>
                </Grid>
                <Grid item xs={3}>

                  <Link to="/content-provide-list" className={classes.link}>
                      <Button
                          variant="contained"
                          size="small"
                          className={classes.button}
                          startIcon={<ArrowBackIcon />}
                        >
                          Back
                        </Button>
                    </Link>
                    {/* <SaveIcon />
                    <DeleteIcon /> */}

                    <Button
                      variant="contained"
                      size="small"
                      className={classes.buttonDelete}
                      startIcon={<DeleteIcon />}
                      onClick={handleClickOpenDelete}
                    >
                      Delete
                    </Button>
                    <Button
                      variant="contained"
                      size="small"
                      className={classes.button}
                      startIcon={<SaveIcon />}
                      onClick={handleClickOpenSave}
                    >
                      Save
                    </Button>
                  
                </Grid>
            </Grid>
                {/* <CommandBar
                    items={_items}
                    farItems={_farItems}
                    ariaLabel="Use left and right arrow keys to navigate between commands"
                /> */}
            </div>
            <Dialog
                open={openDelete}
                onClose={handleCloseDelete}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">{"Do you want to continue?"}</DialogTitle>
                <DialogContent>
                <DialogContentText id="alert-dialog-description">
                    Clicking OK will delete this record. Please click CANCEL to go back.
                </DialogContentText>
                </DialogContent>
                <DialogActions>
                <Button onClick={handleCloseDelete} color="primary">
                    CANCEL
                </Button>
                <Button onClick={handleCloseDelete} color="primary" autoFocus>
                    OK
                </Button>
                </DialogActions>
            </Dialog>
            <Dialog
                open={openSave}
                onClose={handleCloseSave}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">{"Do you want to continue?"}</DialogTitle>
                <DialogContent>
                <DialogContentText id="alert-dialog-description">
                    Please click OK to save the changes and CANCEL to discard the changes
                </DialogContentText>
                </DialogContent>
                <DialogActions>
                <Button onClick={handleCloseSave} color="primary">
                    CANCEL
                </Button>
                <Button onClick={onSubmit} color="primary" autoFocus>
                    OK
                </Button>
                </DialogActions>
            </Dialog>
            <Grid container spacing={3}>
                <Grid item xs={12} sm={12} lg={12}/>
                <Grid item xs={12} sm={12} lg={12}/>
            </Grid>
                
                <Grid container spacing={3}>
                    <Grid item xs={12} sm={4} lg={4}>
                    <TextField
                        id="standard-read-only-input"
                        label="Content Provider Name"
                        defaultValue={props.details.name}
                        variant="outlined"
                        required/>
                    </Grid>
                    <Grid item xs={12} sm={4} lg={4}>
                    <TextField
                        id="standard-read-only-input"
                        label="Contact"
                        defaultValue={props.details.contentAdministrators[0].mobile}
                        variant="outlined"
                        required/>
                    </Grid>
                    <Grid item xs={12} sm={4} lg={4}>
                    <FormControlLabel
                        control={<StatusSwitch checked={props.details.isActive} onChange={handleChange} name="status" />}
                        label="Status"
                    />
                    </Grid>
                    <Grid item xs={12} sm={12} lg={12}>
                    Address
                    </Grid>
                    <Grid item  xs={12} sm={12} lg={12}>
                    <TextField
                        id="outlined-multiline-static"
                        label="Street"
                        fullWidth
                        defaultValue={props.details.address.streetName}
                        variant="outlined"
                        required
                    />
                    </Grid>
                    <Grid item  xs={12} sm={4} lg={4}>
                    <TextField
                        id="outlined-multiline-static"
                        label="City"
                        defaultValue={props.details.address.city}
                        variant="outlined"
                        required
                    />
                    </Grid>
                    <Grid item  xs={12} sm={4} lg={4}>
                    <TextField
                        id="outlined-multiline-static"
                        label="State"
                        defaultValue={props.details.address.state}
                        variant="outlined"
                        required
                    />
                    </Grid>
                    <Grid item  xs={12} sm={4} lg={4}>
                    <TextField
                        id="outlined-multiline-static"
                        name="pincode"
                        onChange={updateValues}
                        label="Pincode"
                        defaultValue={props.details.address.pin}
                        variant="outlined"
                        required
                    />
                    </Grid>
                    <Grid item xs={12} sm={4} lg={4}>
                    <TextField
                        id="standard-read-only-input"
                        name="activationDate"
                        label="Activation Date"
                        defaultValue={props.details.activationDate}
                        variant="outlined"
                        required/>
                    </Grid>
                    <Grid item xs={12} sm={4} lg={4}>
                    <TextField
                        id="standard-read-only-input"
                        label="De-activation Date"
                        defaultValue={props.details.isActive?'NA':props.details.deactivationDate}
                        variant="outlined"
                        required/>
                    </Grid>
                    <Grid item xs={0} sm={4} lg={4}>
                    </Grid>
                    
                    <Grid item xs={12} sm={12} lg={12}>
                    Content Admistrators
                    </Grid>
                    { props.details.contentAdministrators.map(element => 
                        (
                        <Grid item xs={12} sm={3} lg={3}>
                            <Card className={classes.card}>
                              <CardContent>
                                <Typography className={classes.title} color="textSecondary" gutterBottom>
                                    Content Admin
                                </Typography>
                                <Typography variant="h5" component="h2">
                                    {element.firstName+' '+element.middleName+' '+element.lastName }
                                </Typography>
                                <Typography className={classes.pos} color="textSecondary">
                                  {element.mobile}
                                </Typography>
                                <Typography variant="body2" component="p">
                                  {element.email}
                                </Typography>
                                </CardContent>
                                <CardActions>
                                <Button size="small">More Info</Button>
                              </CardActions>
                            </Card>
                        </Grid>  
                        ))                      
                    }
                   
                </Grid>
            </div>
    
    );
  }
}
 