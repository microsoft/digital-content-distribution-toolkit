import React from 'react';
import clsx from 'clsx';
import { makeStyles, useTheme } from '@material-ui/core/styles';
import Drawer from '@material-ui/core/Drawer';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import List from '@material-ui/core/List';
import CssBaseline from '@material-ui/core/CssBaseline';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';
import Divider from '@material-ui/core/Divider';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import AccountCircle from '@material-ui/icons/AccountCircle';
import MenuItem from "@material-ui/core/MenuItem";
import Menu from "@material-ui/core/Menu";
import { Route } from 'react-router';
import { Link } from 'react-router-dom';

import './custom.css';
import HomeIcon from './components/icons/HomeIcon';
import { ContentProviderList } from './components/ContentProvider/ContentProviderList';
import { HubList } from './components/Hubs/HubList';
import { Profile } from './components/User/Profile';
import {Home} from './components/Home';
import {Policies} from './components/Policy/Policies';


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
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
    transition: theme.transitions.create('margin', {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
    marginLeft: -drawerWidth,
  },
  link: {
    textDecoration: 'none',
  },
  container: {
    marginLeft: 'auto',
    marginRight: '7%',
    width: '70%'
  }
}));

export default function PersistentDrawerLeft() {
  const classes = useStyles();
  const theme = useTheme();
  const [open, setOpen] = React.useState(false);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const openAnchor = Boolean(anchorEl);

  const handleDrawerOpen = () => {
    setOpen(true);
  };

  const handleDrawerClose = () => {
    setOpen(false);
  };

  const handleMenu = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  
  const toggleDrawer = (status) => (event) => {
    if (event.key === 'Tab') {
      return;
    }

    setOpen(status);
  };

  return (
    <div className={classes.root}>
      <div onKeyDown={toggleDrawer(false)}>
        <CssBaseline />
        <AppBar
          position="fixed"
        >
          <Toolbar>
            <IconButton
              color="inherit"
              aria-label="open drawer"
              onClick={toggleDrawer(true)}
              edge="start"
              className={clsx(classes.menuButton, open && classes.hide)}
            >
              <MenuIcon />
            </IconButton>
            <Typography variant="h6" noWrap className={classes.title}>
              Bine CRM
            </Typography>
            <div>
              <IconButton
                    aria-label="account of current user"
                    aria-controls="menu-appbar"
                    aria-haspopup="true"
                    onClick={handleMenu}
                    color="inherit"
                  >
                    <AccountCircle />
              </IconButton>
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
                <MenuItem> <Link to="/profile" className={classes.link}>Profile</Link></MenuItem>
                <MenuItem onClick={handleClose}>My account</MenuItem>
              </Menu>
            </div>
          </Toolbar>
        </AppBar>
        <Drawer
          className={classes.drawer}
          variant="persistent"
          anchor="left"
          open={open}
          onClose={toggleDrawer(false)}
          classes={{
            paper: classes.drawerPaper,
          }}
        >
          <div className={classes.drawerHeader}>

            <IconButton onClick={toggleDrawer(false)}>
              <ChevronLeftIcon />
            </IconButton>
          </div>
          <Divider />
          <List>
          <ListItem button key='Home'>
              <ListItemText>
                  <div>
                    
                    <Link to="/home" className={classes.link}><HomeIcon color="secondary"/>Home</Link>
                  </div>
                  
              </ListItemText>
            </ListItem>
            <ListItem button key='Content-Providers'>
              <ListItemText>
                  <Link to="/content-provider-list" className={classes.link}>Content Providers</Link>
              </ListItemText>
            </ListItem>
            <ListItem button key='Hubs'>
                  <ListItemText>
                      <Link  className={classes.link} to="/hubs">Hubs</Link>
                  </ListItemText>
            </ListItem>
            <ListItem button key='Policies' >
                <ListItemText>
                    <Link className={classes.link} to="/policies">Policies</Link>
              </ListItemText>
            </ListItem>
          </List>
          <Divider />
        </Drawer>
        <main
        className={clsx(classes.content, {
          [classes.content]: open,
        })}
      >
        <div className={classes.drawerHeader} />
        <CssBaseline />
          <Container className={classes.container}>
            <Route path='/home' component={Home} />
            <Route path='/content-provider-list' component={ContentProviderList} />
            <Route path='/hubs' component={HubList} />
            <Route path='/policies' component={Policies} />
            <Route path='/profile' component={Profile} />
          </Container>
        
      </main>
      </div>
    </div>
  );
}