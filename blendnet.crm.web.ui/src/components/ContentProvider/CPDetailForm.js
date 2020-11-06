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



export default function DetailForm() {
  const classes = useStyles();
  const [state, setState] = React.useState({
    status: true,
  });

const [openSave, setOpenSave] = React.useState(false);
const [openDelete, setOpenDelete] = React.useState(false);


const handleCloseSave = () => {
  setOpenSave(false);
};

const handleClickOpenSave = () => {
  setOpenSave(true);
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


  
// const _items = [
    
//   ];
  
// const _farItems= [
// {
//     key: 'back',
//     text: 'Back',
//     // This needs an ariaLabel since it's icon-only
//     ariaLabel: 'Back',
//     iconOnly: true,
//     iconProps: { iconName: 'Back' },
//     onClick:  () => {
//         setOpen(true);
//       },
// },
// {
//     key: 'copy',
//     text: 'Copy',
//     // This needs an ariaLabel since it's icon-only
//     ariaLabel: 'Copy',
//     iconOnly: true,
//     iconProps: { iconName: 'Copy' },
//     onClick:  () => {
//         setOpen(true);
//       },
// },
// {
//     key: 'save',
//     text: 'Save',
//     // This needs an ariaLabel since it's icon-only
//     ariaLabel: 'Save',
//     iconOnly: true,
//     iconProps: { iconName: 'Save' },
//     onClick: () => {
//         setOpen(true);
//       },
// },
// ];
  
  
  

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
                <Button onClick={handleCloseSave} color="primary" autoFocus>
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
                        defaultValue="ALT Balaji"
                        variant="outlined"
                        required/>
                    </Grid>
                    <Grid item xs={12} sm={4} lg={4}>
                    <TextField
                        id="standard-read-only-input"
                        label="Contact"
                        defaultValue="9665037918"
                        variant="outlined"
                        required/>
                    </Grid>
                    <Grid item xs={12} sm={4} lg={4}>
                    <FormControlLabel
                        control={<StatusSwitch checked={state.status} onChange={handleChange} name="status" />}
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
                        defaultValue="Bandra"
                        variant="outlined"
                        required
                    />
                    </Grid>
                    <Grid item  xs={12} sm={4} lg={4}>
                    <TextField
                        id="outlined-multiline-static"
                        label="City"
                        defaultValue="Mumbai"
                        variant="outlined"
                        required
                    />
                    </Grid>
                    <Grid item  xs={12} sm={4} lg={4}>
                    <TextField
                        id="outlined-multiline-static"
                        label="State"
                        defaultValue="Maharashtra"
                        variant="outlined"
                        required
                    />
                    </Grid>
                    <Grid item  xs={12} sm={4} lg={4}>
                    <TextField
                        id="outlined-multiline-static"
                        label="Pincode"
                        defaultValue="460076"
                        variant="outlined"
                        required
                    />
                    </Grid>
                    <Grid item xs={12} sm={4} lg={4}>
                    <TextField
                        id="standard-read-only-input"
                        label="Activation Date"
                        defaultValue="2020-10-28T08:05:36.789Z"
                        variant="outlined"
                        required/>
                    </Grid>
                    <Grid item xs={12} sm={4} lg={4}>
                    <TextField
                        id="standard-read-only-input"
                        label="De-activation Date"
                        defaultValue="2020-10-28T08:05:36.789Z"
                        variant="outlined"
                        required/>
                    </Grid>
                    <Grid item xs={0} sm={4} lg={4}>
                    </Grid>
                    
                    <Grid item xs={12} sm={12} lg={12}>
                    Content Admistrators
                    </Grid>
                    <Grid item xs={12} sm={3} lg={3}>
                    <Card className={classes.card}>
                        <CardContent>
                        <Typography className={classes.title} color="textSecondary" gutterBottom>
                            Content Admin
                        </Typography>
                        <Typography variant="h5" component="h2">
                            John Doe
                        </Typography>
                        <Typography className={classes.pos} color="textSecondary">
                            +91 9658965896
                        </Typography>
                        <Typography variant="body2" component="p">
                            Mumbai,
                            <br />
                            MH - 460003
                        </Typography>
                        </CardContent>
                        <CardActions>
                        <Button size="small">More Info</Button>
                        </CardActions>
                    </Card>
                    </Grid>
                    <Grid item xs={12} sm={3} lg={3}>
                    <Card className={classes.card}>
                        <CardContent>
                        <Typography className={classes.title} color="textSecondary" gutterBottom>
                          Content Admin
                        </Typography>
                        <Typography variant="h5" component="h2">
                            Jane Doe
                        </Typography>
                        <Typography className={classes.pos} color="textSecondary">
                            +91 9658555896
                        </Typography>
                        <Typography variant="body2" component="p">
                            Worli
                            <br />
                            MH - 465500
                        </Typography>
                        </CardContent>
                        <CardActions>
                        <Button size="small">More Info</Button>
                        </CardActions>
                    </Card>
                    </Grid>
                </Grid>
            </div>
    
  );
}
 