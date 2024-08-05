# Ansible Setup and Docker Installation Guide

## Add SSH Key

Generate a new SSH key:

```bash
ssh-keygen
```

### Setup Host in local machine

Edit the Ansible hosts file:

```

sudo nano /etc/ansible/hosts

```

Add the following configuration to /etc/ansible/hosts:

```

[azure_vms]
my_vm ansible_host=VM_IP ansible_user=raghavendiran ansible_python_interpreter=/usr/bin/python3

[azure_vms:vars]
LANG=en_US.UTF-8
LC_ALL=en_US.UTF-8

```

## Create a New User in Ansible

Create a playbook named create_ansible_user.yml:

```

---

- name: Create Ansible User
  hosts: azure_vms
  become: yes
  tasks:

  - name: Create a new user
    user:
    name: ansible_user
    state: present
    shell: /bin/bash

  - name: Allow 'ansible_user' to have passwordless sudo
    lineinfile:
    path: /etc/sudoers
    state: present
    regexp: '^ansible_user'
    line: 'ansible_user ALL=(ALL) NOPASSWD:ALL'

  - name: Create .ssh directory
    file:
    path: /home/ansible_user/.ssh
    state: directory
    owner: ansible_user
    group: ansible_user
    mode: '0700'

  - name: Add your public key to ansible_user's authorized_keys
    copy:
    src: /home/raghavendiran/.ssh/id_rsa.pub
    dest: /home/ansible_user/.ssh/authorized_keys
    owner: ansible_user
    group: ansible_user
    mode: '0600'

```

Run the playbook:

```

ansible-playbook create_ansible_user.yml

```

### Install Docker

Create a playbook named install_docker.yml:

```

---

- name: Install Docker
  hosts: azure_vms
  become: yes
  tasks:

  - name: Update the apt package index
    apt:
    update_cache: yes

  - name: Install packages to allow apt to use a repository over HTTPS
    apt:
    name: ['apt-transport-https', 'ca-certificates', 'curl', 'software-properties-common']
    state: present

  - name: Add Docker’s official GPG key
    apt_key:
    url: https://download.docker.com/linux/ubuntu/gpg
    state: present

  - name: Add Docker apt repository
    apt_repository:
    repo: deb [arch=amd64] https://download.docker.com/linux/ubuntu {{ ansible_distribution_release }} stable
    state: present

  - name: Install Docker CE
    apt:
    name: docker-ce
    state: present

  - name: Ensure Docker is started
    service:
    name: docker
    state: started
    enabled: yes

```

Run the playbook:

```

ansible-playbook install_docker.yml

```

```

```
