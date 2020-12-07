import React from 'react';
import clsx from 'clsx';
import { makeStyles } from '@material-ui/core/styles';
import Drawer from '@material-ui/core/Drawer';
import Divider from '@material-ui/core/Divider';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import List from '@material-ui/core/List';
import CssBaseline from '@material-ui/core/CssBaseline';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import { IconButton } from '@fluentui/react/lib/Button';

import MenuItem from "@material-ui/core/MenuItem";
import Menu from "@material-ui/core/Menu";
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import './custom.css';
import { ContentProviderList } from './components/ContentProvider/ContentProviderList';
import { HubList } from './components/Hubs/HubList';
import { Profile } from './components/User/Profile';
import {Home} from './components/Home';
import {Policies} from './components/Policy/Policies';
import Tooltip from '@material-ui/core/Tooltip';
import { ContentProviderDetail } from './components/ContentProvider/ContentProviderDetail';
import { Route } from 'react-router';
import { Link } from 'react-router-dom';

const drawerWidth = 240;

const useStyles = makeStyles((theme) => ({
  root: {
    flexGrow: 1,
  },
  title: {
    flexGrow: 1
  },
  menuButton: {
    marginRight: theme.spacing(2),
    color: '#ffffff'
  },
  hide: {
    display: 'none',
  },
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
  },
  drawerPaper: {
    width: drawerWidth,
  },
  drawerHeader: {
    display: 'flex',
    alignItems: 'center',
    padding: theme.spacing(0, 1),
    // necessary for content to be below app bar
    ...theme.mixins.toolbar,
    justifyContent: 'flex-end',
  },
  link: {
    textDecoration: 'none',
  },
  container: {
    marginLeft: 'auto',
    marginRight: '15%',
    marginTop: '5%',
    width: '70%'
  },
  account: {
    color: '#ffffff'
  },
  appBar: {
    backgroundColor: "#0078d4",
  }
}));

export default function TemporaryDrawer() {
  const classes = useStyles();
  const [open, setOpen] = React.useState(false);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const openAnchor = Boolean(anchorEl);
  const [state, setState] = React.useState({
    left: false
  });

  const toggleDrawer = (anchor, open) => (event) => {
    if (event.type === 'keydown' && (event.key === 'Tab' || event.key === 'Shift')) {
      return;
    }

    setState({ ...state, [anchor]: open });
  };

  const handleMenu = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };


  const list = (anchor) => (
    <div
      className={classes.list}
      role="presentation"
      onClick={toggleDrawer(anchor, false)}
      onKeyDown={toggleDrawer(anchor, false)}
    >
        <div className={classes.drawerHeader}>
            <IconButton title="ChevronLeft" ariaLabel="ChevronLeft"   iconProps={{ iconName: 'ChevronLeft' }} />
        </div>
        <Divider></Divider>
        <List>
            <Link to="/home" className={classes.link}>
              <ListItem button key='Home'>
                <ListItemText>
                      <IconButton title="HomeSolid" ariaLabel="HomeSolid"  iconProps={{ iconName: 'HomeSolid' }} /> 
                      Home 
                </ListItemText>
              </ListItem>
            </Link>
            <Link to="/content-provide-list" className={classes.link}>
              <ListItem button key='Home'>
                <ListItemText>
                    <IconButton title="Media" ariaLabel="Media"  iconProps={{ iconName: 'Media' }} />
                      Content-Providers
                </ListItemText>
              </ListItem>
            </Link>
            <Link  className={classes.link} to="/hubs">
              <ListItem button key='Hubs'>
                    <ListItemText>
                        <IconButton title="Cloud" ariaLabel="Cloud"  iconProps={{ iconName: 'Cloud' }} />
                        Hubs
                    </ListItemText>
              </ListItem>
            </Link>
            <Link className={classes.link} to="/policies">
              <ListItem button key='Policies' >
                  <ListItemText>
                      <IconButton title="EntitlementPolicy" ariaLabel="EntitlementPolicy"  iconProps={{ iconName: 'EntitlementPolicy' }} />
                        Policies
                </ListItemText>
              </ListItem>
            </Link>
            
        </List>
        <Divider></Divider>
    </div>
  );

  return (
    <div>
        <React.Fragment>
        <AppBar
          className={classes.appBar}
          position="fixed"
        >
          <Toolbar>
              <IconButton title="Menu" ariaLabel="Menu"  iconProps={{ iconName: 'CollapseMenu' }}  
              aria-label="open drawer"
              onClick={toggleDrawer('left', true)}
              edge="start"
              className={clsx(classes.menuButton, open && classes.hide)}
              />
            <Typography variant="h6" noWrap className={classes.title}>
              Bine CRM
            </Typography>
            <div>
            <Tooltip title="Account">
              <IconButton title="Account" ariaLabel="Account"   iconProps={{ iconName: 'Contact' }} 
              className={classes.account}
              onClick={handleMenu}/>
            </Tooltip>
              <Menu
                id="menu-appbar"
                anchorEl={anchorEl}
                anchorOrigin={{
                  vertical: "top",
                  horizontal: "right"
                }}
                keepMounted
                transformOrigin={{
                  vertical: "top",
                  horizontal: "right"
                }}
                open={openAnchor}
                onClose={handleClose}
                >
                <Link to="/profile" className={classes.link}><MenuItem> Profile</MenuItem></Link>
                <Link to="/profile" className={classes.link}><MenuItem>My account</MenuItem></Link>
              </Menu>
            </div>
          </Toolbar>
        </AppBar>
          <Drawer anchor='left' open={state['left']} onClose={toggleDrawer('left', false)}
          classes={{
            paper: classes.drawerPaper,
          }}>
            {list('left')}
          </Drawer>
        </React.Fragment>
        <div className={classes.drawerHeader} />
        <CssBaseline />
          <Container className={classes.container}>
            <Route path='/content-provide-list' component={ContentProviderList} />
            <Route path='/hubs' component={HubList} />
            <Route path='/profile' component={Profile} />
            <Route path='/home' component={Home} />
            <Route path='/policies' component={Policies} />
            <Route path='/content-provide-detail' component={ContentProviderDetail} />
          </Container>
      </div>
  );
}
